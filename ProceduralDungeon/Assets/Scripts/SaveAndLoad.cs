using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    [SerializeField] private string _fileName = "saveData.json";
    [SerializeField] private string _filePath = "Assets/Resources/Save/";
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
        
        float[,] mapRotation = tr.GetComponent<MapData>().GetMapRotation();

        string data = map.GetLength(0) + ":" + map.GetLength(1) + ":";
        //je parcours toute ma data et je la met dans un string
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
                data += map[i, j] + ":";
        }
        
        string dataRotation = "";
        for (int i = 0; i < mapRotation.GetLength(0); i++)
        {
            for (int j = 0; j < mapRotation.GetLength(1); j++)
            {
                dataRotation += mapRotation[i, j] + ":";
            }
        }
        
        System.IO.Directory.CreateDirectory(_filePath);
        System.IO.File.WriteAllText(_filePath + _fileName, data);
        
        System.IO.File.WriteAllText(_filePath + _fileName + "Rotation", dataRotation);
        Debug.Log("Saved Done");
    }

    [ContextMenu("Load Data")]
    private void Load()
    {
        string data = System.IO.File.ReadAllText(_filePath + _fileName);
        string[] dataSplit = data.Split(':');
        int x = int.Parse(dataSplit[0]);
        int y = int.Parse(dataSplit[1]);
        float[,] map = new float[x, y];
        int index = 2;
        //je parcours toute ma data et je la met dans un string
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++, index++)
                map[i, j] = float.Parse(dataSplit[index]);
        }
        
        string dataRotation = System.IO.File.ReadAllText(_filePath + _fileName + "Rotation");
        string[] dataSplitRotation = dataRotation.Split(':');
        float [,] mapRotation = new float[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                mapRotation[i, j] = float.Parse(dataSplitRotation[i * y + j]);
            }
        }
        
        
        _generateTerrain.SetRoomByName(roomToSaveName, map, mapRotation);
    //la il faut pop le terrain
    }
}