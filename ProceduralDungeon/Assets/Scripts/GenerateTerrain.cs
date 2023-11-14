using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateTerrain : MonoBehaviour
{
    [Header("Terrain Parameters")] [SerializeField]
    private Vector2Int terrainDimensions;

    [SerializeField] private GameObject terrainPrefab;

    [Header("Terrain data")] [SerializeField]
    private List<TileSO> Layers = new();

    [Header("Random parameters")] [SerializeField] [Range(1, 10000)]
    int worldSeed = 1;

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
    private void GenerateTerrainMesh()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            DestroyImmediate(child);
            i--;
        }

        GameObject terrain = Instantiate(terrainPrefab, transform);
        terrain.name = "Terrain";
        MeshFilter meshFilter = terrain.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.vertices = UtilsToolTerrain.GenerateTerrainVertices(terrainDimensions);
        mesh.triangles = UtilsToolTerrain.GenerateTerrainTriangles(terrainDimensions);
        mesh.uv = UtilsToolTerrain.GenerateTerrainUVs(terrainDimensions);
        mesh.RecalculateNormals();
        GenerateData();
    }
}