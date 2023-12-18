using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

public enum Group
{
    All,
    Group1,
    Group2,
    Group3,
    Group4,
    Group5,
}

[ExecuteInEditMode]
public class RESTApiClient : MonoBehaviour
{
    public static RESTApiClient Instance;

    [Header("Set Groupname")]
    [SerializeField]
    private Group _group = Group.All;

    [Header("Base REST-API")]
    [SerializeField] private string baseRemote;
    [SerializeField] private string baseLocal;
    [SerializeField] private bool useLocalServer;
    [SerializeField] private bool debug = false;

    [Header("Data Routes")]
    public string allPlacesRoute = "/api/unity/places/all";
    public string groupSpecificRoute = "/api/unity/places/by-author-name";
    public string uploadSinglePlaceRoute = "/api/unity/places/add";

    private string _placesRouteURL, _uploadSinglePlace;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        string baseURL = useLocalServer ? baseLocal : baseRemote;
        baseURL = RemoveEmptySpaces(baseURL);

        _placesRouteURL = _group == Group.All ? string.Format("{0}{1}", baseURL, allPlacesRoute) : string.Format("{0}{1}/{2}", baseURL, groupSpecificRoute, _group.ToString());
        _uploadSinglePlace = string.Format("{0}{1}", baseURL, uploadSinglePlaceRoute);
    }

    private string RemoveEmptySpaces(string input)
    {
        return Regex.Replace(input, @"\s+", string.Empty); // changed by ppg
    }

    public void GetPlacesFromServer(Action<List<Place>> callback)
    {
        StartCoroutine(ProcessRequest(_placesRouteURL, callback));
    }

    private IEnumerator ProcessRequest(string uri, Action<List<Place>> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            request.disposeDownloadHandlerOnDispose = true;
            
            yield return request.SendWebRequest();

            // no connection at all
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
                callback(null);
            }
            else
            {
                // retrieved wrong data (no valid json response)
                if (request.downloadHandler.text.Contains("!DOCTYPE"))
                {
                    callback(null);
                }
                else
                {
                    try
                    {
                        var places = JsonConvert.DeserializeObject<List<Place>>(request.downloadHandler.text);
                        callback(places);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        callback(null);
                    }
                }
            }
        }

        yield return null;
    }

    // upload photospots data to server
    public void UploadSinglePlace(string data)
    {
        StartCoroutine(UploadData(_uploadSinglePlace, data));
    }

    private IEnumerator UploadData(string uri, string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", data);

        using (UnityWebRequest request = UnityWebRequest.Post(uri, form))
        {
            request.disposeUploadHandlerOnDispose = true;
            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                // Debug.Log(request.downloadHandler.text);
            }
        }
    }

    public Group GetGroupname()
    {
        return _group;
    }
}