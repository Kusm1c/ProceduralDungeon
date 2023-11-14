using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateTerrain))]
public class GenerateTerrainEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate Terrain"))
        {
            GenerateTerrain terrain = (GenerateTerrain) target;
            terrain.GenerateTerrainMesh();
        }
    }
}
