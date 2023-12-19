using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draws the default inspector

        DataManager dataManager = (DataManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test API requests / responses", EditorStyles.boldLabel);
        
        // Add a custom button to the inspector
        if (GUILayout.Button("Add new place"))
        {
            dataManager.AddPlaceToDataBase(RESTApiClient.Instance.GetGroupname(), 0.0, 0.0, 0.0, Quaternion.identity,"Demo Place 2", "Demo Info 2");
        }
        
        // Add a custom button to the inspector
        if (GUILayout.Button("Add new place with texture"))
        {
            dataManager.AddPlaceToDataBase(RESTApiClient.Instance.GetGroupname(), 0.0, 0.0, 0.0, Quaternion.identity, "Demo Place 1", "Demo Info 1", dataManager.GetDemoTexture2D());
        }
        
        // Add a custom button to the inspector
        if (GUILayout.Button("Request all places from server"))
        {
            dataManager.RequestPlacesDataFromServer();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("API Webinterface", EditorStyles.boldLabel);

        // Add a custom button to the inspector
        if (GUILayout.Button("Open in browser"))
        {
            Application.OpenURL("http://141.100.233.223:3000");
        }
    }
}