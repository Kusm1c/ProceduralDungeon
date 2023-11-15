using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsTerrainData
{
    public static bool CheckAllConditions(List<TileSO> layers, int indexLayer, Vector2Int position,
        Vector2Int terrainDimensions, float[,] mapData)
    {
        for (int i = 0; i < layers[indexLayer].conditions.Count; i++)
        {
            if (!CheckCondition(layers[indexLayer].conditions[i], layers, position, terrainDimensions, mapData))
                return false;
        }

        return true;
    }

    private static bool CheckCondition(Condition soCondition, List<TileSO> layers, Vector2Int pos,
        Vector2Int terrainDimensions, float[,] mapData)
    {
        TileSO tile = layers.Find(x => x.type == soCondition.type);

        Vector2Int posCond = (soCondition.position == Position.Top && pos.y < terrainDimensions.y - 1)
            ? pos + Vector2Int.up
            : (soCondition.position == Position.Bottom && pos.y > 0)
                ? pos + Vector2Int.down
                : (soCondition.position == Position.Left && pos.x > 0)
                    ? pos + Vector2Int.left
                    : (soCondition.position == Position.Right && pos.x < terrainDimensions.x - 1)
                        ? pos + Vector2Int.right
                        : (soCondition.position == Position.BottomLeft && pos.y > 0 && pos.x > 0)
                            ? pos + Vector2Int.one * -1
                            : (soCondition.position == Position.BottomRight && pos.y > 0 &&
                               pos.x < terrainDimensions.x - 1)
                                ? pos + Vector2Int.down + Vector2Int.right
                                : (soCondition.position == Position.TopLeft && pos.y < terrainDimensions.y - 1 &&
                                   pos.x > 0)
                                    ? pos + Vector2Int.up + Vector2Int.left
                                    : (soCondition.position == Position.TopRight && pos.y < terrainDimensions.y - 1 &&
                                       pos.x < terrainDimensions.x - 1)
                                        ? pos + Vector2Int.up + Vector2Int.right
                                        : Vector2Int.one * -1;

        if (posCond is { x: -1, y: -1 }) return false;

        return (soCondition.possibility == Possibility.Must)
            ? CheckMustOrNot(posCond, soCondition.type, mapData, true)
            :
            (soCondition.possibility == Possibility.MustNot)
                ? CheckMustOrNot(posCond, soCondition.type, mapData, false)
                :
                (soCondition.possibility == Possibility.Can)
                    ? CheckCanOrCant(posCond, soCondition.type, mapData , true)
                    :
                    (soCondition.possibility == Possibility.Cant)
                        ? CheckCanOrCant(posCond, soCondition.type, mapData , false)
                        :
                        throw new System.Exception("Error in UtilsTerrainData.CheckCondition");
    }

    private static bool CheckCanOrCant(Vector2Int posCond, Type soConditionType, float[,] mapData, bool can)
    {
        bool cond = mapData[posCond.x, posCond.y] == (int)soConditionType;
        return can switch
        {
            true when Random.Range(0, 100) < 25 => true,
            false when Random.Range(0, 100) < 25 => false,
            _ => (can) ? cond : !cond
        };
    }


    private static bool CheckMustOrNot(Vector2Int pos, Type type, float[,] mapData, bool isMust)
    {
        //Debug.Log(pos.y + " , " + pos.x);
        bool cond = mapData[pos.x, pos.y] == (int)type;
        return (isMust) ? cond : !cond;
    }
}