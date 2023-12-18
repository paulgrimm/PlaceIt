using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draws the default inspector

        DataManager dataManager = (DataManager)target;

        // Add a custom button to the inspector
        if (GUILayout.Button("Add Place with Texture"))
        {
            dataManager.AddPlaceToDataBase(RESTApiClient.Instance.GetGroupname(), 0.0, 0.0, 0.0, Quaternion.identity, "Demo Place 1", "Demo Info 1", dataManager.GetDemoTexture2D());
        }
        
        // Add a custom button to the inspector
        if (GUILayout.Button("Add Place"))
        {
            dataManager.AddPlaceToDataBase(RESTApiClient.Instance.GetGroupname(), 0.0, 0.0, 0.0, Quaternion.identity,"Demo Place 2", "Demo Info 2");
        }
        
        // Add a custom button to the inspector
        if (GUILayout.Button("Request Data from Server"))
        {
            dataManager.RequestPlacesDataFromServer();
        }
    }
}