using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainGeneratorWindow : EditorWindow
{
    private string terrainObjectName = "UwU_Terrain";
    private GameObject generatedTerrainObject;

    private Vector2Int terrainDimensions = new Vector2Int(10, 10);
    private int worldSeed = 123; // Example value
    private bool regenerateAtRuntime = false;
    private bool recookedAtRuntime = false;
    
    [SerializeField] private Transform terrainTransform;
    [SerializeField] private Transform cameraLaveRT;
    [SerializeField] private TileSO Wall;
    [SerializeField] private List<TileSO> Layers = new();

    [Header("Shader Parameters")] [SerializeField]
    private float thicknessParam = 0.95f;

    [SerializeField] private Color gridColor = Color.red;
    [SerializeField] private Color lineColor = Color.white;
    private string pathFloorShader = "ProceduralDungeon/Floor/SG_Floor";
    private string pathTilingInShader = "_Tiling";
    private string pathThicknessInShader = "_LineThickness";
    private string pathGridColorInShader = "_GridColor";
    private string pathLineColorInShader = "_LineGridColor";

    [MenuItem("Window/Terrain Generator")]
    public static void ShowWindow()
    {
        GetWindow<TerrainGeneratorWindow>("Terrain Generator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Terrain Generator", EditorStyles.boldLabel);

        terrainObjectName = EditorGUILayout.TextField("Terrain Object Name", terrainObjectName);

        EditorGUILayout.LabelField("Terrain Dimensions", EditorStyles.boldLabel);
        terrainDimensions = new Vector2Int(
            EditorGUILayout.IntSlider("Terrain Dimensions X", terrainDimensions.x, 1, 100),
            EditorGUILayout.IntSlider("Terrain Dimensions Y", terrainDimensions.y, 1, 100)
        );

        EditorGUILayout.LabelField("Seed Parameters", EditorStyles.boldLabel);
        worldSeed = EditorGUILayout.IntSlider("World Seed", worldSeed, 1, 10000);

        EditorGUILayout.LabelField("Runtime Parameters", EditorStyles.boldLabel);
        regenerateAtRuntime = EditorGUILayout.Toggle("Regenerate TerrainData", regenerateAtRuntime);
        EditorGUI.BeginDisabledGroup(!regenerateAtRuntime);
        recookedAtRuntime = EditorGUILayout.Toggle("ReCooked", recookedAtRuntime);
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Generate Terrain"))
        {
            GenerateTerrainObject();
        }

        if (generatedTerrainObject != null)
        {
            EditorGUILayout.LabelField("Terrain Generated: " + generatedTerrainObject.name);
        }
        else
        {
            EditorGUILayout.LabelField("Terrain Generated: None");
        }

        GUI.enabled = generatedTerrainObject != null;
        
        if (GUILayout.Button("Generate Terrain Mesh"))
        {
            GenerateTerrainMesh();
        }

        if (GUILayout.Button("Generate Terrain Data"))
        {
            GenerateTerrainData();
        }

        if (GUILayout.Button("Update Terrain Material"))
        {
            UpdateTerrainMaterial();
        }

        if (GUILayout.Button("Build Nav Mesh"))
        {
            BuilTerrainNavMesh();
        }

        if (GUILayout.Button("Cook 3D World"))
        {
            GenerateTerrain3DWold();
        }

        if (GUILayout.Button("Clear World"))
        {
            ClearTerrainData();
        }

        GUI.enabled = true;
    }

    private void GenerateTerrainObject()
    {
        GameObject terrainObject = new GameObject("Generated Terrain");
        GenerateTerrain generateTerrain = terrainObject.AddComponent<GenerateTerrain>();
        generateTerrain.name = terrainObjectName;
        Selection.activeGameObject = terrainObject;
        generatedTerrainObject = terrainObject;
    }
    
    private void GenerateTerrainMesh()
    {
        GenerateTerrain generateTerrain = generatedTerrainObject.GetComponent<GenerateTerrain>();
        if (generateTerrain != null)
        {
            generateTerrain.GenerateTerrainMesh();
        }
    }    
    
    private void GenerateTerrainData()
    {
        GenerateTerrain generateTerrain = generatedTerrainObject.GetComponent<GenerateTerrain>();
        if (generateTerrain != null)
        {
            generateTerrain.GenerateData();
            EditorUtility.SetDirty(generateTerrain);
        }
    } 
    
    private void UpdateTerrainMaterial()
    {
        GenerateTerrain generateTerrain = generatedTerrainObject.GetComponent<GenerateTerrain>();
        if (generateTerrain != null)
        {
            generateTerrain.UpdateMaterial();
            EditorUtility.SetDirty(generateTerrain);
        }
    }   
    
    private void BuilTerrainNavMesh()
    {
        GenerateTerrain generateTerrain = generatedTerrainObject.GetComponent<GenerateTerrain>();
        if (generateTerrain != null)
        {
            generateTerrain.BuildNavMesh();
            EditorUtility.SetDirty(generateTerrain);
        }
    } 
    
    
    private void GenerateTerrain3DWold()
    {
        GenerateTerrain generateTerrain = generatedTerrainObject.GetComponent<GenerateTerrain>();
        if (generateTerrain != null)
        {
            generateTerrain.Generate3DWorld();
            EditorUtility.SetDirty(generateTerrain);
        }
    }  
    
    
    private void ClearTerrainData()
    {
        GenerateTerrain generateTerrain = generatedTerrainObject.GetComponent<GenerateTerrain>();
        if (generateTerrain != null)
        {
            generateTerrain.ClearWorld();
            EditorUtility.SetDirty(generateTerrain);
        }
    }
}