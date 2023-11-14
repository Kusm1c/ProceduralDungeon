using System;
using System.Collections.Generic;
using System.IO;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateTerrain : MonoBehaviour
{
   [Header("Terrain Parameters")]
   [SerializeField] private Vector2Int terrainDimensions;
   [SerializeField] private Transform terrainTransform;
   [SerializeField] private Transform cameraLaveRT;
   [SerializeField] private TileSO Wall;
   [SerializeField] private List<TileSO> Layers = new ();
   
   [Header("Shader Parameters")]
   [SerializeField] private float thicknessParam = 0.95f;
   [SerializeField] private Color gridColor = Color.red ;
   [SerializeField] private Color lineColor = Color.white ;
   private string pathFloorShader = "ProceduralDungeon/Floor/SG_Floor";
   private string pathTilingInShader = "_Tiling";
   private string pathThicknessInShader = "_LineThickness";
   private string pathGridColorInShader = "_GridColor";
   private string pathLineColorInShader = "_LineGridColor";
   
   //Reférence Object
   [HideInInspector]public GameObject terrainRef;
   [HideInInspector]public GameObject cameraLavaRef;
   
   [Header("Random parameters")]
   [SerializeField] [Range(1, 10000)] private int worldSeed = 1;
   [SerializeField] private bool useRandomSeed = false;
   
   private float[,] mapData;
   private List<Vector2Int> AvailablePositions = new();
   private Random.State stateBeforeStep3;
   
   private Dictionary<int, TileSO> _dicTileSO = new ();

    private void Start()
    {
        GenerateTerrainMesh();
        GenerateData();
        BuildNavMesh();
        PlayerManager.instance.SpawnPlayer();
    }

    private void BuildNavMesh()
    {
        NavMeshSurface navMeshSurface = terrainRef.GetComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
    }

    private GameObject GenerateTile(GameObject prefab, Transform parent, Vector3 pos, Material _mat)
    {
        GameObject go = Instantiate(prefab, parent);
        go.transform.position = pos;
        go.GetComponent<Renderer>().material = _mat;
        return go;
    }

    private void ChoosePosToUse(TileSO so, List<Vector2Int> positions)
    {
        int numToUse = (positions.Count > so.numMaxGenerated) ? so.numMaxGenerated : positions.Count;
        int min = (positions.Count < so.numMinGenerated) ? positions.Count : so.numMinGenerated;

        numToUse = Random.Range(min, numToUse + 1);
        GameObject goP = new GameObject
        {
            name = so.type.ToString()
        };
        goP.transform.parent = transform;
        
        for (int i = 0; i < numToUse; i++)
        {
            int index = Random.Range(0, positions.Count);
            Vector2Int pos = positions[index];
            AvailablePositions.Remove(pos);
            mapData[pos.x, pos.y] = (int)so.type;
            positions.RemoveAt(index);
            
            GenerateTile(so.tilePrefab, goP.transform, new Vector3(pos.x, 0.1f, pos.y), so.color2D);
        }
    }

    public void Generate3DWorld()
    {
        for (int i = 0; i < terrainDimensions.y; i++)
        {
            for (int j = 0; j < terrainDimensions.x; j++)
            {
                TileSO so = _dicTileSO[(int)mapData[j, i]];
                if (so.Model3D_S1.Count > 0)
                {
                    int index = Random.Range(0, so.Model3D_S1.Count);
                    GameObject go = Instantiate(so.Model3D_S1[index], transform);
                    go.transform.position = new Vector3(j, 0, i);
                }
            }
        }
    }
    
    public void GenerateData()
    {
        AvailablePositions.Clear();
        if (!useRandomSeed)
            Random.InitState(worldSeed);
        List<Vector2Int> unavailablePositions = new List<Vector2Int>();
        UtilsToolTerrain.InitData(ref mapData, ref AvailablePositions, terrainDimensions,
            ref  unavailablePositions);
        
        Debug.Log("Initial mapData values:");
        for (int y = 0; y < terrainDimensions.y; y++)
        {
            for (int x = 0; x < terrainDimensions.x; x++)
            {
                Debug.Log($"mapData[{y}, {x}] = {mapData[y, x]}");
            }
        }

        GameObject goP = new GameObject
        {
            name = Wall.type.ToString()
        };
        goP.transform.parent = transform;
        
        //_dicTileSO.Add((int)Wall.type, Wall);

        for (int i = 0; i < unavailablePositions.Count; i++)
        {
           GenerateTile(Wall.tilePrefab, goP.transform, 
               new Vector3(unavailablePositions[i].x, 0.1f, unavailablePositions[i].y), Wall.color2D);
        }

        List<Vector2Int> ValidPositions = new List<Vector2Int>();

        for (int i = 0; i < Layers.Count; i++)
        {
            ValidPositions.Clear();
            //_dicTileSO.Add((int)Layers[i].type, Layers[i]);
            for (int j = 0; j < AvailablePositions.Count; j++)
            {
                if (UtilsTerrainData.CheckAllConditions(Layers, i, AvailablePositions[j], terrainDimensions, mapData))
                    ValidPositions.Add(AvailablePositions[j]);
            }
            ChoosePosToUse(Layers[i], ValidPositions);
        }
    }
    
    [ContextMenu("Generate Terrain")]
    public void GenerateTerrainMesh()
    {
        //Get Transform
        GameObject terrain = Instantiate(terrainTransform.gameObject, transform);
        terrain.name = "Terrain";

        //VAO Component
        terrain.AddComponent<MeshFilter>();
        MeshFilter meshFilter = terrain.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        meshFilter.sharedMesh.name = "ME_Terrain_01";
        mesh.vertices = UtilsToolTerrain.GenerateSimpleFloorVertices(terrainDimensions);
        mesh.triangles = UtilsToolTerrain.GenerateSimpleFloorTriangles();
        mesh.uv = UtilsToolTerrain.GenerateSimpleFloorUV(terrainDimensions);
        mesh.RecalculateNormals();
       
        //RendererPart
        terrain.AddComponent<MeshRenderer>();
        MeshRenderer meshRenderer = terrain.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find(pathFloorShader));
        meshRenderer.sharedMaterial.name = "M_Terrain_01";
        
        //Setup Camera
        SetupCameraLava();

        //Setup Shader 
        meshRenderer.sharedMaterial.SetFloat(pathThicknessInShader, thicknessParam);
        meshRenderer.sharedMaterial.SetVector(pathTilingInShader, new Vector4(terrainDimensions.x, terrainDimensions.y, 0, 0));
        meshRenderer.sharedMaterial.SetColor(pathGridColorInShader, gridColor);
        meshRenderer.sharedMaterial.SetColor(pathLineColorInShader, lineColor);
       
        terrainRef = terrain;
        
        //Setup terrain Collider
        terrain.AddComponent<MeshCollider>();
        MeshCollider meshCollider = terrain.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        
        //NavMesh
        terrain.AddComponent<NavMeshSurface>();
    }
    

    [ContextMenu("Update Terrain Material")]
   public void UpdateMaterial()
   {
       MeshRenderer meshRenderer = terrainRef.GetComponent<MeshRenderer>();
       meshRenderer.sharedMaterial.SetFloat(pathThicknessInShader, thicknessParam);
       meshRenderer.sharedMaterial.SetVector(pathTilingInShader, new Vector4(terrainDimensions.x, terrainDimensions.y, 0, 0));
       meshRenderer.sharedMaterial.SetColor(pathGridColorInShader, gridColor);
       meshRenderer.sharedMaterial.SetColor(pathLineColorInShader, lineColor);
   }

   public void SetupCameraLava()
   {
       GameObject cameraLava = Instantiate(cameraLaveRT.gameObject, transform);
       cameraLava.transform.position = new Vector3((float)terrainDimensions.x / 2, 5, (float)terrainDimensions.y / 2);
       cameraLava.GetComponent<Camera>().orthographicSize = (float)(terrainDimensions.x + terrainDimensions.y) / 4;
       cameraLavaRef = cameraLava;
   }
}