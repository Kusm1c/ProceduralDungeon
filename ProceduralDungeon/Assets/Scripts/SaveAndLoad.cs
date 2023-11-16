using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    [SerializeField] private string _fileName = "saveData.json";
    [SerializeField] private string _filePath = "Assets/";
    [SerializeField] private string roomToSaveName;

    [Space(10)] [SerializeField] private GenerateTerrain _generateTerrain;

    [ContextMenu("Save Data")]
    private void Save()
    {
        Transform tr = _generateTerrain.GetRoomByName(roomToSaveName);
        if (!tr)
        {
            Debug.LogError("Room not found");
            return;
        }
        float[,] map = tr.GetComponent<MapData>().GetMap();

        string data = map.GetLength(0) + ":" + map.GetLength(1) + ":";
        //je parcours toute ma data et je la met dans un string
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
                data += map[i, j] + ":";
        }
        return;
        
        
        System.IO.Directory.CreateDirectory(_filePath);
        System.IO.File.WriteAllText(_filePath + _fileName, data);
        Debug.Log("Saved Done");
    }

    [ContextMenu("Load Data")]
    private void Load()
    {
        string data = System.IO.File.ReadAllText(_filePath + _fileName);
        float[,] map = JsonUtility.FromJson<float[,]>(data);

    //la il faut pop le terrain
    }
}