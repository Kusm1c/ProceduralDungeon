using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsToolTerrain
{
    public static Vector2[] GenerateTerrainUVs(Vector2 terrainDimensions)
    {
        int numVerticesX = (int)terrainDimensions.x + 1;
        int numVerticesZ = (int)terrainDimensions.y + 1;

        Vector2[] uvs = new Vector2[numVerticesX * numVerticesZ];
        int index = 0;

        for (int x = 0; x < numVerticesX; x++)
        {
            for (int z = 0; z < numVerticesZ; z++)
            {
                uvs[index] = new Vector2((float)x / terrainDimensions.x, (float)z / terrainDimensions.y);
                index++;
            }
        }

        return uvs;
    }

    
    public static int[] GenerateTerrainTriangles(Vector2 terrainDimensions)
    {
        int numVerticesX = (int)terrainDimensions.x + 1;
        int numVerticesZ = (int)terrainDimensions.y + 1;

        int[] triangles = new int[(numVerticesX - 1) * (numVerticesZ - 1) * 6];
        int triangleIndex = 0;
        int vertexIndex = 0;

        for (int x = 0; x < numVerticesX - 1; x++)
        {
            for (int z = 0; z < numVerticesZ - 1; z++)
            {
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 1;
                triangles[triangleIndex + 2] = vertexIndex + numVerticesZ;
                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + 1 + numVerticesZ;
                triangles[triangleIndex + 5] = vertexIndex + numVerticesZ;

                triangleIndex += 6;
                vertexIndex++;
            }

            vertexIndex++;
        }

        return triangles;
    }


    //TODO : Subdivision
    public static Vector3[] GenerateTerrainVertices(Vector2 terrainDimensions)
    {
        int numVerticesX = (int)terrainDimensions.x + 1;
        int numVerticesZ = (int)terrainDimensions.y + 1;

        Vector3[] vertices = new Vector3[numVerticesX * numVerticesZ];
        int index = 0;

        for (int x = 0; x < numVerticesX; x++)
        {
            for (int z = 0; z < numVerticesZ; z++)
            {
                vertices[index] = new Vector3(x, 0, z);
                index++;
            }
        }

        return vertices;
    }
}