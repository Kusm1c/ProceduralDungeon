using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateTerrain : MonoBehaviour
{
    [Header("Terrain Parameters")] [HideInInspector] [SerializeField]
    private Vector2Int terrainDimensions;

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

    //Reférence Object
    [HideInInspector] public GameObject terrainRef;
    [HideInInspector] public GameObject cameraLavaRef;

    [Header("Random parameters")] [HideInInspector] [SerializeField] [Range(1, 10000)]
    private int worldSeed = 1;

    [SerializeField] private bool useRandomSeed = false;

    [HideInInspector][SerializeField] public float[,] mapData;
    private List<Vector2Int> AvailablePositions = new();
    private Random.State stateBeforeStep3;

    private Dictionary<int, TileSO> _dicTileSO = new();
    [HideInInspector] [SerializeField] private bool regenerateAtRuntime = false;
    [HideInInspector] [SerializeField] private bool recookedAtRuntime = false;

    private void Start()
    {
        GenerateTerrainMesh();
        GenerateData();
        Generate3DWorld();
        BuildNavMesh();
        PlayerManager.instance.SpawnPlayer();
    }

    public void BuildNavMesh()
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
        GameObject goP = new GameObject
        {
            name = "Generated 3DWorld"
        };
        goP.transform.parent = transform;


        float[,] mapDataSave = new float[terrainDimensions.x, terrainDimensions.y];
        for (int x = 0; x < terrainDimensions.x; x++)
        {
            for (int y = 0; y < terrainDimensions.y; y++)
            {
                mapDataSave[x, y] = mapData[x, y];
            }
        }

        for (int x = 0; x < terrainDimensions.x; x++)
        {
            for (int y = 0; y < terrainDimensions.y; y++)
            {
                _dicTileSO.TryGetValue((int)mapDataSave[x, y], out TileSO so);
                if (so == null) continue;

                bool XisGood = true;

                int posTile = 0;
                int indexTile = (int)mapDataSave[x, y];
                if (indexTile != -1 && so.TillingTextureModel)
                    while (posTile + y < terrainDimensions.y && indexTile == mapDataSave[x, y + posTile])
                    {
                        if ((posTile + y + 1 < terrainDimensions.y && indexTile == mapDataSave[x, y + posTile + 1]) ||
                            posTile > 0)
                            mapDataSave[x, y + posTile] = -1;

                        posTile++;
                    }

                if (posTile <= 1 && so.TillingTextureModel)
                {
                    XisGood = false;
                    posTile = 0;
                    indexTile = (int)mapDataSave[x, y];
                    if (indexTile != -1)
                        while (posTile + x < terrainDimensions.x && indexTile == mapDataSave[x + posTile, y])
                        {
                            if ((posTile + x + 1 < terrainDimensions.x &&
                                 indexTile == mapDataSave[x + posTile + 1, y]) || posTile > 0)
                                mapDataSave[x + posTile, y] = -1;
                            posTile++;
                        }

                    if (posTile <= 1)
                        XisGood = true;
                }

                GameObject go = null;
                if (!XisGood && so.TillingTextureModel && so.Model3D_S1.Count > 0) // horizontal scale
                {
                    int index = Random.Range(0, so.Model3D_S1.Count);

                    go = Instantiate(so.Model3D_S1[index], goP.transform);

                    Vector3 scale = go.transform.localScale;
                    scale.x += (float)posTile;
                    go.transform.localScale = scale;
                    go.transform.position = new Vector3((x + posTile) / 2f, scale.y / 2f, y);
                }

                else if (XisGood && so.TillingTextureModel && so.Model3D_S1.Count > 0) // vertical scale
                {
                    int index = Random.Range(0, so.Model3D_S1.Count);
                    go = Instantiate(so.Model3D_S1[index], goP.transform);

                    Vector3 scale = go.transform.localScale;
                    scale.x += (float)posTile;
                    go.transform.localScale = scale;
                    go.transform.position = new Vector3(x, scale.y / 2f, (y + posTile) / 2f);
                    //go.transform.Rotate(Vector3.up, 90);
                }

                else if (XisGood && so.Model3D_S1.Count > 0) // just one tile
                {
                    int index = Random.Range(0, so.Model3D_S1.Count);

                    go = Instantiate(so.Model3D_S1[index], goP.transform);
                    go.transform.position = new Vector3(x, transform.localScale.y / 2f, y);
                }
                if ((x == 0 && y == 0) || (x == terrainDimensions.x - 1 && y == 1) || so.RotationModel3D)
                    go.transform.Rotate(Vector3.up, 90);
            }
        }
    }

    private void ResetData()
    {
        AvailablePositions.Clear();
        _dicTileSO.Clear();
        mapData = new float[terrainDimensions.x, terrainDimensions.y];
    }

    private void ClearData()
    {
        AvailablePositions.Clear();
        _dicTileSO.Clear();
        mapData = null;
    }

    public void GenerateData()
    {
        ResetData();

        if (!useRandomSeed)
            Random.InitState(worldSeed);
        List<Vector2Int> unavailablePositions = new List<Vector2Int>();
        UtilsToolTerrain.InitData(ref mapData, ref AvailablePositions, terrainDimensions,
            ref unavailablePositions);

        GameObject goP = new GameObject
        {
            name = Wall.type.ToString()
        };
        goP.transform.parent = transform;

        _dicTileSO.Add((int)Wall.type, Wall);

        for (int i = 0; i < unavailablePositions.Count; i++)
        {
            GenerateTile(Wall.tilePrefab, goP.transform,
                new Vector3(unavailablePositions[i].x, 0.1f, unavailablePositions[i].y), Wall.color2D);
        }

        List<Vector2Int> ValidPositions = new List<Vector2Int>();

        for (int i = 0; i < Layers.Count; i++)
        {
            ValidPositions.Clear();
            _dicTileSO.Add((int)Layers[i].type, Layers[i]);
            ValidPositions.AddRange(AvailablePositions.Where(t => UtilsTerrainData.CheckAllConditions(Layers, i, t, terrainDimensions, mapData)));

            ChoosePosToUse(Layers[i], ValidPositions);
        }
    }

    [ContextMenu("Generate Terrain")]
    public void GenerateTerrainMesh()
    {
        ClearWorld();

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
        meshRenderer.sharedMaterial.SetVector(pathTilingInShader,
            new Vector4(terrainDimensions.x, terrainDimensions.y, 0, 0));
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
        meshRenderer.sharedMaterial.SetVector(pathTilingInShader,
            new Vector4(terrainDimensions.x, terrainDimensions.y, 0, 0));
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

    public void ClearWorld()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
            i--;
        }
        ClearData();
    }
}