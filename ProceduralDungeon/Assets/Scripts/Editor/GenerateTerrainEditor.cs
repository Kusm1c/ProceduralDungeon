using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateTerrain))]
public class GenerateTerrainEditor : Editor
{
    SerializedProperty terrainDimensions;
    SerializedProperty worldSeed;
    SerializedProperty regenerateAtRuntime;

    private void OnEnable()
    {
        terrainDimensions = serializedObject.FindProperty("terrainDimensions");
        regenerateAtRuntime = serializedObject.FindProperty("regenerateAtRuntime");
        worldSeed = serializedObject.FindProperty("worldSeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GenerateTerrain terrain = (GenerateTerrain)target;
        EditorGUILayout.LabelField("Terrain Dimensions", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck(); // Start change check

        
        Vector2Int newDimensions = new Vector2Int(
            EditorGUILayout.IntSlider("Terrain Dimensions X", terrainDimensions.vector2IntValue.x, 1, 100),
            EditorGUILayout.IntSlider("Terrain Dimensions Y", terrainDimensions.vector2IntValue.y, 1, 100)
        );

        EditorGUILayout.LabelField("Seed Parameters", EditorStyles.boldLabel);
        int newWorldSeed = EditorGUILayout.IntSlider("World Seed", worldSeed.intValue, 1, 10000);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Regenerate At Runtime", EditorStyles.boldLabel);
        regenerateAtRuntime.boolValue = EditorGUILayout.Toggle(regenerateAtRuntime.boolValue);
        EditorGUILayout.EndHorizontal();
        

        if (EditorGUI.EndChangeCheck())
        { 
            worldSeed.intValue = newWorldSeed;
            terrainDimensions.vector2IntValue = newDimensions;
            serializedObject.ApplyModifiedProperties();
            
            if (regenerateAtRuntime.boolValue)
            {
                terrain.GenerateTerrainMesh();
                terrain.GenerateData();
                terrain.BuildNavMesh();
                EditorUtility.SetDirty(terrain);
            }
        }

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
        
        if (GUILayout.Button("Build Nav Mesh"))
        {
            terrain.BuildNavMesh();
            Debug.Log("Update Material");
        }

        serializedObject.ApplyModifiedProperties(); // Move this outside of the if condition
        base.OnInspectorGUI();
    }
}
