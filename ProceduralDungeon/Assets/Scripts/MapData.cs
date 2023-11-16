using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    [HideInInspector] private float[,] _mapData;
    
    public void CopyMap(float[,] map)
    {
        _mapData = new float[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                _mapData[i, j] = map[i, j];
            }
        }
    }
    
    public float[,] GetMap()
    {
        return _mapData;
    }
    
    public void SetMap(float[,] map)
    {
        CopyMap(map);
    }
    
    public void SetMapAtPosition(float value, int x, int y)
    {
        _mapData[x, y] = value;
    }
}
