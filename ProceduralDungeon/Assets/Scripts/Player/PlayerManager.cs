using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public NavMeshSurface surface { get; set; }
    [SerializeField] private GameObject player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform spawnPoint;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SpawnPlayer()
    {
        GameObject go = Instantiate(player, spawnPoint.position, Quaternion.identity);
        mainCamera.GetComponent<CameraScript>().player = go;
        SetPlayersNavMeshAgent();
    }

    private void SetPlayersNavMeshAgent()
    {
        player.GetComponent<NavMeshAgent>().enabled = true;
    }
}