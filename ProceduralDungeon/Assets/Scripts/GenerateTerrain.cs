using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GenerateTerrain : MonoBehaviour
{
   [Header("Terrain Parameters")]
   [SerializeField] private Vector2 terrainDimensions = new (50, 50);
   //[SerializeField] [Range(1,10)] int subdivisionLevel = 1; //TODO : Subdivision
   [SerializeField] private Transform terrainTransform;
   
   [SerializeField] private List<TileSO> Layers = new ();
   
   

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
      mesh.vertices = UtilsToolTerrain.GenerateTerrainVertices(terrainDimensions);
      mesh.triangles = UtilsToolTerrain.GenerateTerrainTriangles(terrainDimensions);
      mesh.uv = UtilsToolTerrain.GenerateTerrainUVs(terrainDimensions);
      mesh.RecalculateNormals();
      
      //RendererPart
      terrain.AddComponent<MeshRenderer>();
      MeshRenderer meshRenderer = terrain.GetComponent<MeshRenderer>();
      meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
      meshRenderer.sharedMaterial.name = "M_Terrain_01";
   }


}
