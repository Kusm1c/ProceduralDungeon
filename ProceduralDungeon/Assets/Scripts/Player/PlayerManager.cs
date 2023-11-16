using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public NavMeshSurface surface { get; set; }
    [SerializeField] private GameObject playerPrefab;
    private GameObject player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform spawnPoint;
    public GenerateTerrain generator;
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
        player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        player.GetComponent<PlayerControl>().generateTerrain = generator;
        mainCamera.GetComponent<CameraScript>().player = player;
        SetPlayersNavMeshAgent();
    }
    
    public void SpawnPlayer(Vector3 position)
    {
        if (player != null)
        {
            Destroy(player);
        }
        player = Instantiate(playerPrefab, position, Quaternion.identity);
        player.GetComponent<PlayerControl>().generateTerrain = generator;
        mainCamera.GetComponent<CameraScript>().player = player;
        SetPlayersNavMeshAgent();
    }


    private void SetPlayersNavMeshAgent()
    {
        playerPrefab.GetComponent<NavMeshAgent>().enabled = true;
    }
}