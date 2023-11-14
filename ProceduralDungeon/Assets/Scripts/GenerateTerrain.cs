using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GenerateTerrain : MonoBehaviour
{
   [Header("Terrain Parameters")]
   [SerializeField] private Vector2 terrainDimensions = new (50, 50);
   [SerializeField] private GameObject terrainPrefab;
   
   [SerializeField] private List<TileSO> Layers = new ();
   
   

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
   }


}
