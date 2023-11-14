using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateTerrain))]
public class GenerateTerrainEditor : Editor
{
    SerializedProperty terrainDimensionsProp;

    private void OnEnable()
    {
        terrainDimensionsProp = serializedObject.FindProperty("terrainDimensions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        GenerateTerrain terrain = (GenerateTerrain)target;

        if (GUILayout.Button("Generate Terrain"))
        {
            terrain.GenerateTerrainMesh();
        }

        if (GUILayout.Button("Generate Terrain Data"))
        {
            serializedObject.ApplyModifiedProperties();

            terrain.GenerateData();
            EditorUtility.SetDirty(terrain);
        }

        if (GUILayout.Button("Update Terrain Material"))
        {
            terrain.UpdateMaterial();
            Debug.Log("Update Material");
        }
    }
}