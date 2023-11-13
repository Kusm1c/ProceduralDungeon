using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSO", menuName = "ScriptableObjects/TileSO", order = 1)]
public class TIleSO : ScriptableObject
{
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private int numMaxGenerated;
    [SerializeField] private Type type;
    [SerializeField] private List<Condition> conditions;
}
