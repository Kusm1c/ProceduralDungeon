using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class UtilsDoors
{
    private GenerateTerrain generateTerrain;
    public bool isFirstRoom = true;
    private Dictionary<DoorSide,Vector2Int> positionOfDoors = new();
    private bool[] sideWithDoor = new bool[4];
    private GameObject door2DParent;
    private GameObject door3DParent;

    public UtilsDoors(GenerateTerrain generateTerrain)
    {
        this.generateTerrain = generateTerrain;
    }

    public void GenerateDoorsData(DoorSide mendatoryDoor)
    {
        positionOfDoors.Clear();
        generateTerrain.nextRoomTilePosition.Clear();
        sideWithDoor = new[]{false,false,false,false};
        
        if (door2DParent) Object.DestroyImmediate(door2DParent);
        door2DParent = new GameObject
        {
            name = "Door 2D",
            transform =
            {
                parent = generateTerrain.transform
            }
        };
        
        if (mendatoryDoor == DoorSide.None)
        {
            int numDoors = Random.Range(1, 5);
            AddDoors(numDoors);
            
            foreach (var door in positionOfDoors)
            {
                PlaceDoor(door, true);
            }
            
            isFirstRoom = false;
        }
        else
        {
            int numDoors = Random.Range(1, 4);
            sideWithDoor[(int)mendatoryDoor] = true;
            AddDoors(numDoors);
            
            foreach (var door in positionOfDoors)
            {
                PlaceDoor(door, true);
            }
        }
    }

    private void AddDoors(int numDoors)
    {
        for (int i = 0; i < numDoors; i++)
        {
            int side = Random.Range(0, 4);
            if (sideWithDoor[side])
            {
                i--;
                continue;
            }

            sideWithDoor[side] = true;
        }
        
        foreach (var door in sideWithDoor.Select((value, index) => new {value, index}))
        {
            if (door.value)
            {
                positionOfDoors.Add((DoorSide)door.index, door.index switch
                {
                    0 => new Vector2Int(Random.Range(1, generateTerrain.terrainDimensions.x - 1), 0),
                    1 => new Vector2Int(Random.Range(1, generateTerrain.terrainDimensions.x - 1), generateTerrain.terrainDimensions.y - 1),
                    2 => new Vector2Int(0, Random.Range(1, generateTerrain.terrainDimensions.y - 1)),
                    3 => new Vector2Int(generateTerrain.terrainDimensions.x - 1, Random.Range(1, generateTerrain.terrainDimensions.y - 1)),
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
        }
    }

    public void GenerateDoors3D()
    {
        if (door3DParent) Object.DestroyImmediate(door3DParent);
        door3DParent = new GameObject
        {
            name = "Door 3D",
            transform =
            {
                parent = generateTerrain.transform
            }
        };
        foreach (var door in positionOfDoors)
        {
            PlaceDoor(door, false);
        }
    }

    private void PlaceDoor(KeyValuePair<DoorSide, Vector2Int> positionOfDoor, bool is2D)
    {
        GameObject go = Object.Instantiate((Object)(is2D ? generateTerrain.doorPrefab2D : generateTerrain.doorPrefab3D), generateTerrain.transform) as GameObject;
        go.transform.position = new Vector3(positionOfDoor.Value.x, 0.1f, positionOfDoor.Value.y);
        switch (positionOfDoor.Key)
        {
            case DoorSide.Top:
                go.transform.rotation = Quaternion.Euler(0, 0, 0);
                if (is2D) generateTerrain.nextRoomTilePosition.Add(new Vector2(go.transform.position.x, go.transform.position.z + 1));
                break;
            case DoorSide.Bottom:
                go.transform.rotation = Quaternion.Euler(0, 180, 0);
                if (is2D) generateTerrain.nextRoomTilePosition.Add(new Vector2(go.transform.position.x, go.transform.position.z - 1));
                break;
            case DoorSide.Left:
                go.transform.rotation = Quaternion.Euler(0, 90, 0);
                if (is2D) generateTerrain.nextRoomTilePosition.Add(new Vector2(go.transform.position.x + 1, go.transform.position.z));
                break;
            case DoorSide.Right:
                go.transform.rotation = Quaternion.Euler(0, 270, 0);
                if (is2D) generateTerrain.nextRoomTilePosition.Add(new Vector2(go.transform.position.x - 1, go.transform.position.z));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        go.transform.parent = is2D ? door2DParent.transform : door3DParent.transform;
    }

    public void GenerateNextRoom(DoorSide directionDoor)
    {
        DisableCurrentRoom();
        int roomIndex = EnableRandomRoom();
        GenerateDoorsData(generateTerrain.rooms.Count == 0 ? DoorSide.None : directionDoor);
        GenerateDoors3D();
        generateTerrain.BuildNavMesh();
        SpawnPlayerAtDoorPosition(directionDoor);
    }

    private void SpawnPlayerAtDoorPosition(DoorSide directionDoor)
    {
        Vector3 pos = Vector3.zero;
        Vector3[] doorPositions = new Vector3[4];
        
        doorPositions[0] = positionOfDoors.ContainsKey(DoorSide.Top) ? new Vector3(positionOfDoors[DoorSide.Top].x, 0, positionOfDoors[DoorSide.Top].y + 2) : Vector3.zero;
        doorPositions[1] = positionOfDoors.ContainsKey(DoorSide.Bottom) ? new Vector3(positionOfDoors[DoorSide.Bottom].x, 0, positionOfDoors[DoorSide.Bottom].y - 2) : Vector3.zero;
        doorPositions[2] = positionOfDoors.ContainsKey(DoorSide.Left) ? new Vector3(positionOfDoors[DoorSide.Left].x + 2, 0, positionOfDoors[DoorSide.Left].y) : Vector3.zero;
        doorPositions[3] = positionOfDoors.ContainsKey(DoorSide.Right) ? new Vector3(positionOfDoors[DoorSide.Right].x - 2, 0, positionOfDoors[DoorSide.Right].y) : Vector3.zero;
        
        pos = doorPositions[(int)directionDoor];

        Debug.Log(pos);
        PlayerManager.instance.SpawnPlayer(pos);
    }

    private void DisableCurrentRoom()
    {
        generateTerrain.rooms[generateTerrain.currentRoom].gameObject.SetActive(false);
    }

    private int EnableRandomRoom()
    {
        generateTerrain.currentRoom = Random.Range(0, generateTerrain.rooms.Count);
        generateTerrain.rooms[generateTerrain.currentRoom].gameObject.transform.position = new Vector3(0,0,0);
        generateTerrain.terrainDimensions = new Vector2Int((int)generateTerrain.rooms[generateTerrain.currentRoom].transform.GetChild(0).GetChild(0).transform.localScale.x,
            (int)generateTerrain.rooms[generateTerrain.currentRoom].transform.GetChild(0).GetChild(0).transform.localScale.z);
        generateTerrain.rooms[generateTerrain.currentRoom].gameObject.SetActive(true);
        return generateTerrain.currentRoom;
    }

    private void OnDrawGizmos()
    {
        foreach (var tile in generateTerrain.nextRoomTilePosition)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(tile.x,0,tile.y), 0.5f);
        }
    }
}