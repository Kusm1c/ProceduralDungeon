using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsToolTerrain
{
    public static Vector2[] GenerateTerrainUVs(Vector2 terrainDimensions)
    {
        Vector2[] uvs = new Vector2[(int)(terrainDimensions.x * terrainDimensions.y)];
        int index = 0;
        for (int x = 0; x < terrainDimensions.x; x++)
        {
            for (int z = 0; z < terrainDimensions.y; z++)
            {
                uvs[index] = new Vector2((float)x / terrainDimensions.x, (float)z / terrainDimensions.y);
                index++;
            }
        }

        return uvs;
    }

    public static int[] GenerateTerrainTriangles(Vector2 terrainDimensions)
    {
        int[] triangles = new int[(int)(terrainDimensions.x * terrainDimensions.y * 6)];
        int triangleIndex = 0;
        int vertexIndex = 0;
        for (int x = 0; x < terrainDimensions.x - 1; x++)
        {
            for (int z = 0; z < terrainDimensions.y - 1; z++)
            {
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 1;
                triangles[triangleIndex + 2] = vertexIndex + (int)terrainDimensions.y;
                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + 1 + (int)terrainDimensions.y;
                triangles[triangleIndex + 5] = vertexIndex + (int)terrainDimensions.y;
                triangleIndex += 6;
                vertexIndex++;
            }

            vertexIndex++;
        }

        return triangles;
    }

    public static Vector3[] GenerateTerrainVertices(Vector2 terrainDimensions)
    {
        Vector3[] vertices = new Vector3[(int)(terrainDimensions.x * terrainDimensions.y)];
        int index = 0;
        for (int x = 0; x < terrainDimensions.x; x++)
        {
            for (int z = 0; z < terrainDimensions.y; z++)
            {
                vertices[index] = new Vector3(x, 0, z);
                index++;
            }
        }

        return vertices;
    }
}