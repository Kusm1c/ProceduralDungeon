using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSO", menuName = "ScriptableObjects/TileSO", order = 1)]
public class TileSO : ScriptableObject
{
    [field:SerializeField] public GameObject tilePrefab{ get; private set; }
    
    [field:SerializeField] public List<GameObject> Model3D_S1{ get; private set; }
    
    [field:SerializeField] public List<GameObject> Model3D_S2{ get; private set; }
    
    [field:SerializeField] public bool TillingTextureModel { get; private set; }
    
    [field:SerializeField] public int numMaxGenerated{ get; private set; }
    [field:SerializeField] public int numMinGenerated{ get; private set; }
    [field:SerializeField] public Type type{ get; private set; }
    [field:SerializeField] public List<Condition> conditions { get; private set; }
    [field:SerializeField] public Material color2D{ get; private set; }
}
