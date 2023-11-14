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
   
   [Header("Shader Parameters")]
   [SerializeField] private float thicknessParam = 0.95f;
   [SerializeField] private Color gridColor = Color.red ;
   [SerializeField] private Color lineColor = Color.white ;
   
   //Private
   private string pathFloorShader = "ProceduralDungeon/Floor/SG_Floor";
   private string pathTilingInShader = "_Tiling";
   private string pathThicknessInShader = "_LineThickness";
   private string pathGridColorInShader = "_GridColor";
   private string pathLineColorInShader = "_LineColor";
   [HideInInspector]public GameObject terrainRef;


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
       meshRenderer.sharedMaterial.SetVector(pathTilingInShader, terrainDimensions);
       meshRenderer.sharedMaterial.SetColor(pathGridColorInShader, gridColor);
       meshRenderer.sharedMaterial.SetColor(pathLineColorInShader, lineColor);

       terrainRef = terrain;
   }
   
   [ContextMenu("Update Terrain Material")]
   public void UpdateMaterial()
   {
       MeshRenderer meshRenderer = terrainRef.GetComponent<MeshRenderer>();
       meshRenderer.sharedMaterial.SetFloat(pathThicknessInShader, thicknessParam);
       meshRenderer.sharedMaterial.SetVector(pathTilingInShader, terrainDimensions);
       //meshRenderer.sharedMaterial.SetColor(pathGridColorInShader, gridColor);
       meshRenderer.sharedMaterial.SetColor(pathLineColorInShader, lineColor);
   }
}
