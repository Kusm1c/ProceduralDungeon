using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    [Header("Terrain Parameters")] [SerializeField]
    private Vector2Int terrainDimensions = new(50, 50);

    [SerializeField] private GameObject terrainPrefab;

    [SerializeField] private List<TileSO> Layers = new();

    private float[,] mapData;
    private List<Vector2Int> AvailablePositions = new();
    [SerializeField] [Range(1, 10000)] int worldSeed = 1;

    private bool CheckAllConditions(TileSO so, Vector2Int position)
    {
        for (int i = 0; i < so.conditions.Count; i++)
        {
            if (!CheckCondition(so.conditions[i], so, position)) return false;
        }

        return true;
    }

    private bool CheckMustOrNot(Vector2Int pos, Type type, bool isMust)
    {
        bool cond = mapData[pos.x, pos.y] == (int)type;
        return (isMust) ? cond : !cond;
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
        }
    }

    private bool CheckCondition(Condition soCondition, TileSO so, Vector2Int pos)
    {
        TileSO tile = Layers.Find(x => x.type == soCondition.type);

        Vector2Int posCond = (soCondition.position == Position.Top && pos.y < terrainDimensions.y - 1)
            ?
            pos + Vector2Int.up
            :
            (soCondition.position == Position.Bottom && pos.y > 0)
                ? pos + Vector2Int.down
                :
                (soCondition.position == Position.Left && pos.x > 0)
                    ? pos + Vector2Int.left
                    :
                    (soCondition.position == Position.Right && pos.x < terrainDimensions.x - 1)
                        ? pos + Vector2Int.right
                        :
                        pos;

        return (soCondition.possibility == Possibility.Must) ? CheckMustOrNot(posCond, soCondition.type, true) :
            (soCondition.possibility == Possibility.MustNot) ? CheckMustOrNot(posCond, soCondition.type, false) :
            throw new System.Exception("Error");
    }

    private void GenerateData()
    {
        Random.InitState(worldSeed);
        UtilsToolTerrain.InitData(ref mapData, ref AvailablePositions, terrainDimensions);

        List<Vector2Int> ValidPositions = new List<Vector2Int>();

        for (int i = 0; i < Layers.Count; i++)
        {
            ValidPositions.Clear();
            for (int j = 0; j < AvailablePositions.Count; j++)
            {
                if (CheckAllConditions(Layers[i], AvailablePositions[j]))
                    ValidPositions.Add(AvailablePositions[j]);
            }

            ChoosePosToUse(Layers[i], ValidPositions);
        }
    }

    [ContextMenu("Generate Terrain")]
    private void GenerateTerrainMesh()
    {
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