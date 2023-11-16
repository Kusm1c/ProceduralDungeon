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
    [HideInInspector] [SerializeField] private List<TileSO> Layers = new();
    [SerializeField] private List<Transform> positionsNotAvailable = new();

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

    [SerializeField] private GameObject doorPrefab;

    [Header("Random parameters")] [HideInInspector] [SerializeField] [Range(1, 10000)]
    private int worldSeed = 1;

    [SerializeField] private bool useRandomSeed = false;
    
    [SerializeField] private bool multipleDoorsOnSameSide = false;

    [HideInInspector] [SerializeField] public float[,] mapData;
    private List<Vector2Int> AvailablePositions = new();
    private Random.State stateBeforeStep3;

    private Dictionary<int, TileSO> _dicTileSO = new();
    [HideInInspector] [SerializeField] private bool regenerateAtRuntime = false;
    [HideInInspector] [SerializeField] private bool recookedAtRuntime = false;

    [Header("Multi room parameters")] [SerializeField]
    private bool useMultiRoom = false;

    [SerializeField] private int numRoom = 5;
    [SerializeField] private int minSizeRoom = 5;
    [SerializeField] private int maxSizeRoom = 10;
    [SerializeField] private int offsetSeed = 10;
    [SerializeField] private int distanceBetweenRoom = 5;
    private GameObject rootParent;


    private void Start()
    {
        GenerateMultiRooms();
    }

    public void BuildNavMesh()
    {
        NavMeshSurface navMeshSurface = terrainRef.GetComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
    }

    private GameObject GenerateTile(GameObject prefab, Transform parent, Vector3 pos, Material _mat)
    {
        GameObject go = Instantiate(prefab, parent);

        go.transform.position = pos + parent.transform.position;
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
        goP.transform.parent = rootParent.transform;
        goP.transform.localPosition = Vector3.zero;

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
        goP.transform.parent = rootParent.transform;
        goP.transform.localPosition = Vector3.zero;

        float[][] mapDataSave = new float[terrainDimensions.x][];
        for (int index = 0; index < terrainDimensions.x; index++)
        {
            mapDataSave[index] = new float[terrainDimensions.y];
        }

        for (int x = 0; x < terrainDimensions.x; x++)
        {
            for (int y = 0; y < terrainDimensions.y; y++)
            {
                mapDataSave[x][y] = mapData[x, y];
            }
        }

        for (int x = 0; x < terrainDimensions.x; x++)
        {
            for (int y = 0; y < terrainDimensions.y; y++)
            {
                _dicTileSO.TryGetValue((int)mapDataSave[x][y], out TileSO so);
                if (so == null) continue;

                int posTileX = 0;
                int posTileY = 0;
                int indexTile = (int)mapDataSave[x][y];
                if (indexTile != -1 && so.TillingTextureModel)
                    while (posTileX + y < terrainDimensions.y && indexTile == mapDataSave[x][y + posTileX])
                    {
                        if ((posTileX + y + 1 < terrainDimensions.y && indexTile == mapDataSave[x][y + posTileX + 1]) ||
                            posTileX > 0)
                            mapDataSave[x][y + posTileX] = -1;

                        posTileX++;
                    }

                if (posTileX < 2)
                {
                    indexTile = (int)mapDataSave[x][y];
                    if (indexTile != -1)
                        while (posTileY + x < terrainDimensions.x && indexTile == mapDataSave[x + posTileY][y])
                        {
                            if ((posTileY + x + 1 < terrainDimensions.x &&
                                 indexTile == mapDataSave[x + posTileY + 1][y]) || posTileY > 0)
                                mapDataSave[x + posTileY][y] = -1;
                            posTileY++;
                        }
                }

                GameObject go = null;
                int index = 0;
                Vector3 scale = Vector3.one;
                if ((so.TillingTextureModel && so.Model3D_S1.Count > 0) ||
                    (posTileX <= 1 && posTileY <= 1 && !so.TillingTextureModel))
                {
                    index = Random.Range(0, so.Model3D_S1.Count);
                    go = Instantiate(so.Model3D_S1[index], goP.transform);
                    go.transform.position = new Vector3(x + goP.transform.position.x,
                        transform.localScale.y * so.CustomOffsetY, y + goP.transform.position.z);
                    scale = go.transform.localScale;
                }

                if (posTileY > 2 && so.TillingTextureModel && so.Model3D_S1.Count > 0) // horizontal scale
                {
                    scale.x += (float)posTileY;
                    go.transform.position = new Vector3((x + posTileY) * 0.5f + goP.transform.position.x,
                        scale.y * so.CustomOffsetY, y + goP.transform.position.z);
                }

                else if (posTileX > 2 && so.TillingTextureModel && so.Model3D_S1.Count > 0) // vertical scale
                {
                    scale.x = (float)posTileX + 1;

                    go.transform.position = new Vector3(x + goP.transform.position.x, scale.y * so.CustomOffsetY,
                        (y + posTileX) * 0.5f + goP.transform.position.z);
                }

                else if (posTileX > 1)
                {
                    int index2 = posTileX;
                    if (so.Model3D_S2.Count > 0)
                    {
                        int nbToSpawn = Mathf.RoundToInt((index2 % 2 == 1 ? index2 - 1 : index2) * 0.5f);
                        index = Random.Range(0, so.Model3D_S2.Count);
                        for (int i = 0; i < nbToSpawn; i++)
                        {
                            go = Instantiate(so.Model3D_S2[index], goP.transform);
                            go.transform.position = new Vector3(x + goP.transform.position.x,
                                transform.localScale.y * so.CustomOffsetY, y + goP.transform.position.z);
                            go.transform.position += new Vector3(0, 0, i * 2 + 0.5f);
                            scale = go.transform.localScale;
                        }

                        index2 -= nbToSpawn * 2;
                    }

                    if (index2 > 0 && so.Model3D_S1.Count > 0)
                    {
                        int position = posTileX - index2;
                        for (int i = 0; i < index2; i++)
                        {
                            index = Random.Range(0, so.Model3D_S1.Count);
                            go = Instantiate(so.Model3D_S1[index], goP.transform);
                            go.transform.position = new Vector3(x + goP.transform.position.x,
                                go.transform.localScale.y * so.CustomOffsetY, y + goP.transform.position.z);
                            go.transform.position += new Vector3(0, 0, position + i);
                            scale = go.transform.localScale;
                        }
                    }
                }

                else if (posTileY > 1)
                {
                    int index2 = posTileY;
                    if (so.Model3D_S2.Count > 0)
                    {
                        int nbToSpawn = Mathf.RoundToInt((index2 % 2 == 1 ? index2 - 1 : index2) * 0.5f);
                        index = Random.Range(0, so.Model3D_S2.Count);
                        for (int i = 0; i < nbToSpawn; i++)
                        {
                            go = Instantiate(so.Model3D_S2[index], goP.transform);
                            go.transform.position = new Vector3(x + goP.transform.position.x,
                                go.transform.localScale.y * so.CustomOffsetY, y + goP.transform.position.z);
                            go.transform.position += new Vector3(i * 2 + 0.5f, 0, 0);
                            scale = go.transform.localScale;
                        }

                        index2 -= nbToSpawn * 2;
                    }

                    if (index2 > 0 && so.Model3D_S1.Count > 0)
                    {
                        int position = posTileY - index2;
                        for (int i = 0; i < index2; i++)
                        {
                            index = Random.Range(0, so.Model3D_S1.Count);
                            go = Instantiate(so.Model3D_S1[index], goP.transform);
                            go.transform.position = new Vector3(x + goP.transform.position.x,
                                go.transform.localScale.y * so.CustomOffsetY, y + goP.transform.position.z);
                            go.transform.position += new Vector3(position + i, 0, 0);
                            scale = go.transform.localScale;
                        }
                    }
                }

                else if (so.Model3D_S1.Count > 0) // just one tile
                {
                    go.transform.position = new Vector3(x + goP.transform.position.x,
                        go.transform.localScale.y * so.CustomOffsetY, y + goP.transform.position.z);
                }

                go.transform.localScale = scale;
                if ((x == 0) || (x == terrainDimensions.x - 1) || so.RotationModel3D)
                    go.transform.Rotate(Vector3.up, !so.RotationModel3D ? 90 : Random.Range(0, 4) * 90);
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

    private void AddNewRootParent(int index)
    {
        rootParent = new GameObject
        {
            name = "Room " + index
        };
        rootParent.transform.parent = transform;
        Debug.Log("Room dimensions : " + terrainDimensions);
        Vector3 newPos = new Vector3(terrainDimensions.x * index, 0, 0);
        newPos += Vector3.right * (index > 0 ? distanceBetweenRoom : 0);
        Debug.Log("newPos " + index + " : " + newPos);

        rootParent.transform.position = newPos;
        
    }

    private void GenerateMultiRooms()
    {
        List<Transform> rooms = new List<Transform>();

        int nbRoomToSpawn = useMultiRoom ? numRoom : 1;
        for (int i = 0; i < nbRoomToSpawn; i++)
        {
            if (useMultiRoom)
                RandomizeSizeRoom();
            AddNewRootParent(i);
            rootParent.transform.parent = rootParent.transform;
            GenerateTerrainMesh();
            GenerateData();
            Generate3DWorld();
            BuildNavMesh();
            worldSeed += offsetSeed;
            rooms.Add(rootParent.transform);
            //rootParent.gameObject.SetActive(false);
        }

        PlayerManager.instance.SpawnPlayer();
    }

    private void RandomizeSizeRoom()
    {
        terrainDimensions = new Vector2Int(Random.Range(minSizeRoom, maxSizeRoom),
            Random.Range(minSizeRoom, maxSizeRoom));
    }

    public void GenerateData()
    {
        ResetData();

        if (!useRandomSeed)
            Random.InitState(worldSeed);
        List<Vector2Int> unavailablePositions = new List<Vector2Int>();
        UtilsToolTerrain.InitData(ref mapData, ref AvailablePositions, terrainDimensions,
            ref unavailablePositions);
        GenerateDoors();
        for (int i = 0; i < positionsNotAvailable.Count; i++)
        {
            mapData[(int)positionsNotAvailable[i].position.x, (int)positionsNotAvailable[i].position.z] = 99;
            if (AvailablePositions.Contains(new Vector2Int((int)positionsNotAvailable[i].position.x,
                    (int)positionsNotAvailable[i].position.z)))
                AvailablePositions.Remove(new Vector2Int((int)positionsNotAvailable[i].position.x,
                    (int)positionsNotAvailable[i].position.z));
        }

        //corner
        GameObject goPCorner = new GameObject
        {
            name = Layers[0].type.ToString()
        };
        goPCorner.transform.parent = rootParent.transform;
        goPCorner.transform.localPosition = Vector3.zero;

        _dicTileSO.Add((int)Layers[0].type, Layers[0]);
        //wall
        GameObject goPWall = new GameObject
        {
            name = Layers[1].type.ToString()
        };
        goPWall.transform.parent = rootParent.transform;
        goPWall.transform.localPosition = Vector3.zero;

        _dicTileSO.Add((int)Layers[1].type, Layers[1]);

        for (int i = 0; i < unavailablePositions.Count; i++)
        {
            Transform tr = null;
            Material mat = null;
            if (unavailablePositions[i] == Vector2Int.zero ||
                unavailablePositions[i] == new Vector2Int(terrainDimensions.x - 1, 0) ||
                unavailablePositions[i] == new Vector2Int(0, terrainDimensions.y - 1) ||
                unavailablePositions[i] == new Vector2Int(terrainDimensions.x - 1, terrainDimensions.y - 1)
               )
            {
                tr = goPCorner.transform;
                mat = Layers[0].color2D;
            }
            else
            {
                tr = goPWall.transform;
                mat = Layers[1].color2D;
            }

            GenerateTile(Layers[1].tilePrefab, tr,
                new Vector3(unavailablePositions[i].x, 0.1f, unavailablePositions[i].y), mat);
        }

        List<Vector2Int> ValidPositions = new List<Vector2Int>();

        for (int i = 2; i < Layers.Count; i++)
        {
            ValidPositions.Clear();
            _dicTileSO.Add((int)Layers[i].type, Layers[i]);
            ValidPositions.AddRange(AvailablePositions.Where(t =>
                UtilsTerrainData.CheckAllConditions(Layers, i, t, terrainDimensions, mapData)));

            ChoosePosToUse(Layers[i], ValidPositions);
        }
    }

    [ContextMenu("Generate Terrain")]
    public void GenerateTerrainMesh()
    {
        if (!useMultiRoom)
            ClearWorld();
        if (!rootParent)
            AddNewRootParent(0);
        //Get Transform
        GameObject terrain = Instantiate(terrainTransform.gameObject, rootParent.transform);
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

        rootParent = null;

        ClearData();
    }

    private bool isFirstRoom = true;
    private List<Vector2Int> positionOfDoors = new();
    private int[] sideWithDoor = new int[4];

    public void GenerateDoors()
    {
        if (isFirstRoom)
        {
            int numDoors = Random.Range(1, 5);
            for (int i = 0; i < numDoors; i++)
            {
                int x = Random.Range(0, terrainDimensions.x);
                int y = Random.Range(0, terrainDimensions.y);
                if (x == 0 || x == terrainDimensions.x - 1 || y == 0 || y == terrainDimensions.y - 1)
                {
                    if (!multipleDoorsOnSameSide)
                    {
                        PlaceDoor(new Vector2Int(x, y));
                    }
                    else
                    {
                        if (x == 0 && y == 0 || x == 0 && y == terrainDimensions.y - 1 ||
                            x == terrainDimensions.x - 1 && y == 0 ||
                            x == terrainDimensions.x - 1 && y == terrainDimensions.y - 1)
                        {
                            i--;
                        }
                        else
                        {
                            positionOfDoors.Add(new Vector2Int(x, y));
                            PlaceDoor(new Vector2Int(x, y));
                        }
                    }
                }
                else
                {
                    i--;
                }
            }

            // isFirstRoom = false;
        }
        else
        {
            //set a door directly next to the previous room's door then generate 1 to 3 more doors on the other sides
            int index = Random.Range(0, positionOfDoors.Count);
            Vector2Int pos = positionOfDoors[index];
        }
    }

    private void PlaceDoor(Vector2Int positionOfDoor)
    {
        GameObject go = Instantiate(doorPrefab, transform);
        go.transform.position = new Vector3(positionOfDoor.x, 0.1f, positionOfDoor.y);
    }
}