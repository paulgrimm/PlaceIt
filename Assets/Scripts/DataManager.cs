using System;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions.Samples.Geospatial;
using Newtonsoft.Json;
using UnityEngine;

// Classes for the json deserialization, followes the structure of the json from the server
public class Place
{
    public int id { get; set; }
    public string name { get; set; }
    public string info { get; set; }
    public string url { get; set; }
    public string base64texture { get; set; }
    public string base64custom { get; set; }
    public string customText { get; set; }
    public int rating { get; set; }
    public Location[] locations { get; set; }
    public Autor autor { get; set; }
}

public class Autor
{
    public int id { get; set; }
    public string name { get; set; }
    public int rating { get; set; }
}

public class Location
{
    public int id { get; set; }
    public double lng { get; set; }
    public double lat { get; set; }
    public double lev { get; set; }
    public float rX { get; set; }
    public float rY { get; set; }
    public float rZ { get; set; }
    public float rW { get; set; }
    public int placeId { get; set; }
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // needed as reference to the geospatialController to call the function to generate a new anchor
    [SerializeField] private GeospatialController geospatialController;
    [SerializeField] private GameObject demoGameObject;
    [SerializeField] private Texture2D demoTexture;

    public bool debug = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    private void Start()
    {
#if UNITY_EDITOR
        if (debug)
        {
            SerializeObjectJson();
        }
#endif
    }


    // retrieves all places from the server and generates them
    public void RequestPlacesDataFromServer()
    {
        // Callback to request the Data from the server
        RESTApiClient.Instance.GetPlacesFromServer(placesFromServer =>
        {
            if (placesFromServer == null || placesFromServer.Count == 0)
            {
                Debug.Log("Error loading Places or Empty");
                return;
            }

            // processes the data from the server
            GeneratePlaces(placesFromServer);
            Debug.Log("Loaded data remotely from PlaceIT-Api", this);
        });
    }


    private void GeneratePlaces(List<Place> placesFromServer)
    {
        foreach(var place in placesFromServer)
        {
            Debug.Log(place.name);
            //GameObject gltfObject = new GameObject();
            //var gltf = gltfObject.AddComponent<GLTFast.GltfAsset>();

            // here goes the go url from place.url
            //gltf.Url = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";

            // convert base64Texture from response to Texture2D
            var newGo = Instantiate(demoGameObject);
            
            if (!place.base64texture.Equals(""))
            {
                byte[] imageData = Convert.FromBase64String(place.base64texture);

                Texture2D texture = new Texture2D(1024, 1024);
                texture.filterMode = FilterMode.Trilinear;
                texture.LoadImage(imageData);
                newGo.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
            }

            foreach (var location in place.locations)
            {
                // CALLS a function from the GeospatialController.cs to generate a new anchor
                // This function has to be implemented in the GeospatialController.cs
                // demoGameObject is a placeholder for the gltfObject (must be available in runtime, functionalliy to download the gltf from the server is not implemented yet)

                var newQuaternion = new Quaternion();
                newQuaternion.x = location.rX;
                newQuaternion.y = location.rY;
                newQuaternion.z = location.rZ;
                newQuaternion.w = location.rW;

                //geospatialController.GenerateNewAnchor(location.lat, location.lng, location.lev, newQuaternion, demoGameObject);
                // CALLS a function from the GeospatialController.cs to generate a new anchor
                // This function has to be implemented in the GeospatialController.cs
                // demoGameObject is a placeholder for the gltfObject (must be available in runtime, functionalliy to download the gltf from the server is not implemented yet)
                geospatialController.PlaceFixedGeospatialAnchor(new GeospatialAnchorHistory(DateTime.Now, location.lat, location.lng, location.lev, AnchorType.Terrain, newQuaternion), newGo);
            }
        }
    }


    // AddPlace including Texture2D Data that was captured before with your application. Texture2D automatically is converted to a base64String
    // customBase64 is a custom field to submit any kind of data as base64String (conversion has to be done manually)
    public void AddPlaceToDataBase(Group authorName, double lng, double lat, double lev, Quaternion rotation, string placeName,
        string placeInfo, Texture2D texture, string objectURL = "", string customText = "", string customBase64 = "")
    {
        // settings
        var setting = new JsonSerializerSettings();
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        var newAutor = new Autor();
        newAutor.name = authorName.ToString();

        var newLocation = new Location();
        newLocation.lng = lng;
        newLocation.lat = lat;
        newLocation.lev = lev;
        newLocation.rX = rotation.x;
        newLocation.rY = rotation.y;
        newLocation.rZ = rotation.z;
        newLocation.rW = rotation.w;

        var newPlace = new Place();
        newPlace.name = placeName;
        newPlace.info = placeInfo;
        newPlace.url = objectURL;

        // generates an image out of the Texture2D and converts it to a base64String to store it in the db
        var bytes = texture.EncodeToJPG();
        newPlace.base64texture = Convert.ToBase64String(bytes);
        newPlace.base64custom = customBase64;

        newPlace.customText = customText;
        newPlace.locations = new Location[] { newLocation };
        newPlace.autor = newAutor;

        // write
        var serializedJson = JsonConvert.SerializeObject(newPlace, setting);
        Debug.Log(serializedJson);

        RESTApiClient.Instance.UploadSinglePlace(serializedJson);
    }


    // authorName must be 'Group1', 'Group2', 'Group3', 'Group4', 'Group5' or 'Group6'
    public void AddPlaceToDataBase(Group authorName, double lng, double lat, double lev, Quaternion rotation, string placeName, string placeInfo = "", string objectURL = "", string customText = "")
    {
        // settings
        var setting = new JsonSerializerSettings();
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        var newAutor = new Autor();
        newAutor.name = authorName.ToString();

        var newLocation = new Location();
        newLocation.lng = lng;
        newLocation.lat = lat;
        newLocation.lev = lev;
        newLocation.rX = rotation.x;
        newLocation.rY = rotation.y;
        newLocation.rZ = rotation.z;
        newLocation.rW = rotation.w;

        var newPlace = new Place();
        newPlace.name = placeName;
        newPlace.info = placeInfo;
        newPlace.url = objectURL;
        newPlace.base64texture = "";
        newPlace.base64custom = "";
        newPlace.customText = customText;
        newPlace.locations = new Location[] { newLocation };
        newPlace.autor = newAutor;

        // write
        var serializedJson = JsonConvert.SerializeObject(newPlace, setting);
        Debug.Log(serializedJson);

        RESTApiClient.Instance.UploadSinglePlace(serializedJson);
    }




    // DEBUG FUNCTIONS ---- DEBUG FUNCTIONS ---- DEBUG FUNCTIONS ---- DEBUG FUNCTIONS ---- DEBUG FUNCTIONS ---- DEBUG FUNCTIONS ---- DEBUG FUNCTIONS ---- 

    // allows access to demo texture data from custom editor buttons
    public Texture2D GetDemoTexture2D()
    {
        return demoTexture;
    }

    // this is a test function to serialize a object to json and upload it to the server
    // newAutor.name = "Andreas" is a forced field, because the server needs a autor and "Andreas" is a fixed placeholder
    private void SerializeObjectJson()
    {
        // settings
        var setting = new JsonSerializerSettings();
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        var newAutor = new Autor();
        newAutor.name = Group.All.ToString();
        newAutor.rating = 1;

        var newLocation = new Location();
        newLocation.lng = 5.5555;
        newLocation.lat = 40.4444;
        newLocation.lev = 270;
        newLocation.rX = 0.0f;
        newLocation.rY = 0.0f;
        newLocation.rZ = 0.0f;
        newLocation.rW = 0.0f;

        var newPlace = new Place();
        newPlace.name = "Test-Place";
        newPlace.info = "Test-Info";
        newPlace.url = "https://anfuchs.de";
        newPlace.base64texture = "";
        newPlace.base64custom = "";
        newPlace.customText = "";
        newPlace.locations = new Location[] { newLocation };
        newPlace.autor = newAutor;

        // write
        var serializedJson = JsonConvert.SerializeObject(newPlace, setting);
        Debug.Log(serializedJson);

        RESTApiClient.Instance.UploadSinglePlace(serializedJson);
    }
}
