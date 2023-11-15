using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsTerrainData
{
    public static bool CheckAllConditions(List<TileSO> layers, int indexLayer, Vector2Int position,
        Vector2Int terrainDimensions, float[,] mapData)
    {
        // if (layers[indexLayer].type == Type.Lava) Debug.Log("num Conditions " + (layers[indexLayer].conditions.Count - 1));
        for (int i = 0; i < layers[indexLayer].conditions.Count; i++)
        {
            for (int j = 0; j < layers[indexLayer].conditions[i].position.Count -1; j++)
            {
                if (!CheckCondition(layers[indexLayer].conditions[i], j,layers[indexLayer], position, terrainDimensions,
                        mapData))
                    return false;
            }
        }
        return true;
    }

    private static bool CheckCondition(Condition soCondition, int index,TileSO tile, Vector2Int pos,
        Vector2Int terrainDimensions, float[,] mapData)
    {
        // if (tile.type == Type.Lava) Debug.Log("index : " + index + " Position " + soCondition.position[index]);
        Vector2Int posCond = (soCondition.position[index] == Position.Top && pos.y < terrainDimensions.y - 1)
            ? pos + Vector2Int.up
            : (soCondition.position[index] == Position.Bottom && pos.y > 0)
                ? pos + Vector2Int.down
                : (soCondition.position[index] == Position.Left && pos.x > 0)
                    ? pos + Vector2Int.left
                    : (soCondition.position[index] == Position.Right && pos.x < terrainDimensions.x - 1)
                        ? pos + Vector2Int.right
                        : (soCondition.position[index] == Position.BottomLeft && pos.y > 0 && pos.x > 0)
                            ? pos + Vector2Int.one * -1
                            : (soCondition.position[index] == Position.BottomRight && pos.y > 0 &&
                               pos.x < terrainDimensions.x - 1)
                                ? pos + Vector2Int.down + Vector2Int.right
                                : (soCondition.position[index] == Position.TopLeft && pos.y < terrainDimensions.y - 1 &&
                                   pos.x > 0)
                                    ? pos + Vector2Int.up + Vector2Int.left
                                    : (soCondition.position[index] == Position.TopRight && pos.y < terrainDimensions.y - 1 &&
                                       pos.x < terrainDimensions.x - 1)
                                        ? pos + Vector2Int.up + Vector2Int.right
                                        : Vector2Int.one * -1;

        if (posCond is { x: -1, y: -1 }) return false;
        // if (tile.type == Type.Lava) Debug.Log("posCond : " + posCond);
        return soCondition.possibility switch
        {
            Possibility.Must => CheckMustOrNot(posCond, soCondition.type, mapData, true),
            Possibility.MustNot => CheckMustOrNot(posCond, soCondition.type, mapData, false),
            Possibility.Can => CheckCanOrCant(posCond, soCondition.type, tile, mapData , true),
            Possibility.Cant => CheckCanOrCant(posCond, soCondition.type, tile, mapData , false),
            _ => throw new System.Exception("Error in UtilsTerrainData.CheckCondition")
        };
    }

    private static bool CheckCanOrCant(Vector2Int posCond, Type soConditionType, TileSO so, float[,] mapData, bool can)
    {
        bool cond = mapData[posCond.x, posCond.y] == (int)soConditionType;
        return can switch
        {
            true when Random.Range(0, 100) < so.canProbability => true,
            false when Random.Range(0, 100) < so.cantProbability => false,
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