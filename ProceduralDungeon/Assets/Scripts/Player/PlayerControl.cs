using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 target;
    private bool isMoving;
    public GenerateTerrain generateTerrain;
    
    [SerializeField] private float speed = 5f;
    
    private float distanceThreshold = 0.1f;
    private float distanceThreshold2 = 1.0f;



    private void MoveTo(Vector3 target)
    {
        this.target = target;
        isMoving = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        isMoving = false;
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastToMousePosition();
        }

        if (!isMoving) return;
        agent.SetDestination(target);
        if (Vector3.Distance(transform.position, target) < distanceThreshold)
        {
            isMoving = false;
        }
        if (generateTerrain.nextRoomTilePosition.Count == 0) return;
        foreach (var doorSide in generateTerrain.nextRoomTilePosition.ToList()
                     .Where(tile => Vector2.Distance(new Vector2(transform.position.x, transform.position.z), tile) <
                                    distanceThreshold2)
                     .Select(tile => generateTerrain.nextRoomTilePosition.IndexOf(tile) switch
                     {
                         0 => DoorSide.Bottom,
                         1 => DoorSide.Top,
                         2 => DoorSide.Right,
                         3 => DoorSide.Left,
                         _ => throw new System.Exception("Error in PlayerControl.Update")
                     }))
        {
            generateTerrain.GenerateNextRoom(doorSide);
        }
    }

    private void RaycastToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 5f);
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            MoveTo(hit.point);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(target, 0.5f);
    }
}
