using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsToolTerrain
{
    public static Vector2[] GenerateTerrainUVs(Vector2 terrainDimensions, int subdivisionLevel)
    {
        int numVerticesX = (int)terrainDimensions.x * subdivisionLevel + 1;
        int numVerticesZ = (int)terrainDimensions.y * subdivisionLevel + 1;

        Vector2[] uvs = new Vector2[numVerticesX * numVerticesZ];
        int index = 0;

        for (int x = 0; x < numVerticesX; x++)
        {
            for (int z = 0; z < numVerticesZ; z++)
            {
                uvs[index] = new Vector2((float)x / (numVerticesX - 1), (float)z / (numVerticesZ - 1));
                index++;
            }
        }

        return uvs;
    }

    public static int[] GenerateTerrainTriangles(Vector2 terrainDimensions, int subdivisionLevel)
    {
        int numVerticesX = (int)terrainDimensions.x * subdivisionLevel + 1;
        int numVerticesZ = (int)terrainDimensions.y * subdivisionLevel + 1;

        int[] triangles = new int[(numVerticesX - 1) * (numVerticesZ - 1) * 6];
        int triangleIndex = 0;
        int vertexIndex = 0;

        for (int x = 0; x < numVerticesX - 1; x++)
        {
            for (int z = 0; z < numVerticesZ - 1; z++)
            {
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + numVerticesZ;
                triangles[triangleIndex + 2] = vertexIndex + 1;

                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + numVerticesZ;
                triangles[triangleIndex + 5] = vertexIndex + numVerticesZ + 1;

                triangleIndex += 6;
                vertexIndex++;
            }

            vertexIndex++;
        }

        return triangles;
    }

    public static Vector3[] GenerateTerrainVertices(Vector2 terrainDimensions, int subdivisionLevel)
    {
        int numVerticesX = (int)terrainDimensions.x * subdivisionLevel + 1;
        int numVerticesZ = (int)terrainDimensions.y * subdivisionLevel + 1;

        Vector3[] vertices = new Vector3[numVerticesX * numVerticesZ];
        int index = 0;

        for (int x = 0; x < numVerticesX; x++)
        {
            for (int z = 0; z < numVerticesZ; z++)
            {
                vertices[index] = new Vector3((float)x / subdivisionLevel, 0, (float)z / subdivisionLevel);
                index++;
            }
        }

        return vertices;
    }
    
    public static Vector3[] GenerateSimpleFloorVertices(Vector2 terrainDimensions)
    {
        Vector3[] vertices = new Vector3[4];
        int index = 0;

        // Set vertices at the specified positions
        vertices[index++] = new Vector3(0, 0, 0);
        vertices[index++] = new Vector3(terrainDimensions.x, 0, 0);
        vertices[index++] = new Vector3(terrainDimensions.x, 0, terrainDimensions.y);
        vertices[index++] = new Vector3(0, 0, terrainDimensions.y);

        return vertices;
    }

    public static Vector2[] GenerateSimpleFloorUV(Vector2 terrainDimensions)
    {
        Vector2[] uvs = new Vector2[4];
    
        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(1, 0);
        uvs[2] = new Vector2(1, 1);
        uvs[3] = new Vector2(0, 1);

        return uvs;
    }

    public static int[] GenerateSimpleFloorTriangles(bool clockwise = true)
    {
        int[] triangles = clockwise ? new int[] { 0, 2, 1, 0, 3, 2 } : new int[] { 0, 1, 2, 0, 2, 3 };
        return triangles;
    }
}