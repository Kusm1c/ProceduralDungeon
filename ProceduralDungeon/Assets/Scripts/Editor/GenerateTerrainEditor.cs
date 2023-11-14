using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateTerrain))]
public class GenerateTerrainEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GenerateTerrain terrain = (GenerateTerrain) target;
        if(GUILayout.Button("Generate Terrain"))
        {
            terrain.GenerateTerrainMesh();
        }
        
        if(GUILayout.Button("Generate Terrain Data"))
        {
            terrain.GenerateData();
        }
        
        if(GUILayout.Button("Update Terrain Material"))
        {
            terrain.UpdateMaterial();
            Debug.Log("Update Material");
        }
    }
}
