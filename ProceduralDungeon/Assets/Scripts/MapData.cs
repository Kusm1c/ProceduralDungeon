using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    [HideInInspector] private float[,] _mapData;
    [HideInInspector] private float[,] _mapRotation;
    
    public void CopyMap(float[,] map, float[,] mapRotation)
    {
        _mapData = new float[map.GetLength(0), map.GetLength(1)];
        _mapRotation = new float[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                _mapData[i, j] = map[i, j];
                _mapRotation[i, j] = mapRotation[i, j];
            }
        }
    }
    
    public float[,] GetMap()
    {
        return _mapData;
    }
    
    public void SetMapAtPosition(float value, int x, int y)
    {
        _mapData[x, y] = value;
    }

    public float[,] GetMapRotation()
    {
        return _mapRotation;
    }
}
