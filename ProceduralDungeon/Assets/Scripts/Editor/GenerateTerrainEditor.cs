using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateTerrain))]
public class GenerateTerrainEditor : Editor
{
    SerializedProperty terrainDimensions;
    SerializedProperty worldSeed;
    SerializedProperty regenerateAtRuntime;
    SerializedProperty recookedAtRuntime;

    private void OnEnable()
    {
        terrainDimensions = serializedObject.FindProperty("terrainDimensions");
        regenerateAtRuntime = serializedObject.FindProperty("regenerateAtRuntime");
        recookedAtRuntime = serializedObject.FindProperty("recookedAtRuntime");
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
        
        EditorGUILayout.LabelField("Runtime Parameters", EditorStyles.boldLabel);
        regenerateAtRuntime.boolValue = EditorGUILayout.Toggle("Regenerate TerrainData", regenerateAtRuntime.boolValue);
        GUI.enabled = regenerateAtRuntime.boolValue;
        recookedAtRuntime.boolValue = EditorGUILayout.Toggle("ReCooked",recookedAtRuntime.boolValue);
        GUI.enabled = true;

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
                if (recookedAtRuntime.boolValue)
                {
                    terrain.Generate3DWorld();
                }
                EditorUtility.SetDirty(terrain);
            }
        }

        if (GUILayout.Button("Generate Terrain"))
        {
            terrain.GenerateTerrainMesh();
        }

        GUI.enabled = terrain.terrainRef != null;
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
        GUI.enabled = true; 

        GUI.enabled = terrain.mapData != null;
        if (GUILayout.Button("Cook 3D World"))
        {
            if (terrain.mapData != null)
            {
                terrain.Generate3DWorld();
                Debug.Log("Cook 3D World");
            }
            else
            {
                Debug.LogError("Cannot cook 3D world. Map data is missing or empty.");
            }
        }
        GUI.enabled = true; 
        
        if (GUILayout.Button("Clear World"))
        {
            terrain.ClearWorld();
            Debug.Log("Clear World");
        }

        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
