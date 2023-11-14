using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateTerrain : MonoBehaviour
{
   [Header("Terrain Parameters")]
   [SerializeField] private Vector2Int terrainDimensions;
   [SerializeField] private Transform terrainTransform;
   [SerializeField] private List<TileSO> Layers = new ();
   [Header("Shader Parameters")]
   [SerializeField] private float thicknessParam = 0.95f;
   [SerializeField] private Color gridColor = Color.red ;
   [SerializeField] private Color lineColor = Color.white ;
   private string pathFloorShader = "ProceduralDungeon/Floor/SG_Floor";
   private string pathTilingInShader = "_Tiling";
   private string pathThicknessInShader = "_LineThickness";
   private string pathGridColorInShader = "_GridColor";
   private string pathLineColorInShader = "_LineColor";
   [HideInInspector]public GameObject terrainRef;
   
   [Header("Random parameters")]
   [SerializeField] [Range(1, 10000)] int worldSeed = 1;

   [SerializeField] bool useRandomSeed = false;


   private float[,] mapData;
   private List<Vector2Int> AvailablePositions = new();
   private Random.State stateBeforeStep3;

    private void Start()
    {
        GenerateTerrainMesh();
    }

    private void ChoosePosToUse(TileSO so, List<Vector2Int> positions)
    {
        int numToUse = (positions.Count > so.numMaxGenerated) ? so.numMaxGenerated : positions.Count;

        numToUse = Random.Range(1, numToUse + 1);
        for (int i = 0; i < numToUse; i++)
        {
            int index = Random.Range(0, positions.Count);
            Vector2Int pos = positions[index];
            AvailablePositions.Remove(pos);
            mapData[pos.x, pos.y] = (int)so.type;
            positions.RemoveAt(index);

            GameObject tile = Instantiate(so.tilePrefab, transform);
            tile.transform.position = new Vector3(pos.x, 0.1f, pos.y);
        }
    }

    private void GenerateData()
    {
        if (!useRandomSeed)
            Random.InitState(worldSeed);
        UtilsToolTerrain.InitData(ref mapData, ref AvailablePositions, terrainDimensions);

        List<Vector2Int> ValidPositions = new List<Vector2Int>();

        for (int i = 0; i < Layers.Count; i++)
        {
            ValidPositions.Clear();
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

        //Setup Shader 
        meshRenderer.sharedMaterial.SetFloat(pathThicknessInShader, thicknessParam);
        meshRenderer.sharedMaterial.SetVector(pathTilingInShader, new Vector4(terrainDimensions.x, terrainDimensions.y, 0, 0));
        meshRenderer.sharedMaterial.SetColor(pathGridColorInShader, gridColor);
        meshRenderer.sharedMaterial.SetColor(pathLineColorInShader, lineColor);
       
        terrainRef = terrain;
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
}