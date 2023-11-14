using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsTerrainData
{
    public static bool CheckAllConditions(List<TileSO> layers, int indexLayer, Vector2Int position, Vector2Int terrainDimensions, float [,]mapData)
    {
        for (int i = 0; i < layers[indexLayer].conditions.Count; i++)
        {
            if (!CheckCondition(layers[indexLayer].conditions[i], layers, position, terrainDimensions, mapData)) return false;
        }

        return true;
    }
    
    private static bool CheckCondition(Condition soCondition, List<TileSO> layers, Vector2Int pos, Vector2Int terrainDimensions, float [,]mapData)
    {
        TileSO tile = layers.Find(x => x.type == soCondition.type);

        Vector2Int posCond = (soCondition.position == Position.Top && pos.y < terrainDimensions.y -1)
            ?
            pos + Vector2Int.up
            :
            (soCondition.position == Position.Bottom && pos.y > 0)
                ? pos + Vector2Int.down
                :
                (soCondition.position == Position.Left && pos.x > 0)
                    ? pos + Vector2Int.left
                    :
                    (soCondition.position == Position.Right && pos.x < terrainDimensions.x - 1)
                        ? pos + Vector2Int.right
                        :
                        Vector2Int.one * -1;
        
        if (posCond is { x: -1, y: -1 }) return false;

        return (soCondition.possibility == Possibility.Must) ? CheckMustOrNot(posCond, soCondition.type, mapData, true) :
            (soCondition.possibility == Possibility.MustNot) ? CheckMustOrNot(posCond, soCondition.type, mapData, false) :
            throw new System.Exception("Error");
    }

    
    private static bool CheckMustOrNot(Vector2Int pos, Type type, float [,]mapData, bool isMust)
    {
        //Debug.Log(pos.y + " , " + pos.x);
        bool cond = mapData[pos.x, pos.y] == (int)type;
        return (isMust) ? cond : !cond;
    }


}
