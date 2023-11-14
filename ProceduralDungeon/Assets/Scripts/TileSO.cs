using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSO", menuName = "ScriptableObjects/TileSO", order = 1)]
public class TileSO : ScriptableObject
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int numMaxGenerated;
    [SerializeField] private Type type;
    [SerializeField] private List<Condition> conditions;
    [SerializeField] private Color color2D;
}
