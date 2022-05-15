using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMapLoader : MonoBehaviour
{
    public int numOfRooms = 20;
    public float dungeonHight = 200;
    public float dungeonWidth = 210;

    public GameObject[] startingRoom;
    public GameObject[] sideWayEnd;
    public GameObject[] bossRoom;

    public GameObject[] upRoomsI;
    public GameObject[] upRoomsTL;
    public GameObject[] upRoomsTR;
    public GameObject[] upRoomsX;

    public GameObject[] rightRoomsT;
    public GameObject[] rightRoomsI;
    public GameObject[] rightRoomsL;
    public GameObject[] rightRoomsEnd;

    public GameObject[] leftRoomsT;
    public GameObject[] leftRoomsI;
    public GameObject[] leftRoomsL;
    public GameObject[] leftRoomsEnd;

    public GameObject[] enemies;
    public GameObject[] objects;

    [HideInInspector]
    public int spawnedRooms = 0;
    public Vector2 bossRoomPos;
    public bool allSpawned = false;

    private RoomManager RM;

    private List<RoomSpawner> activeSpawners = new List<RoomSpawner>();

    private void Start()
    {       
        RoomSpawner.idNumAll = 0;

        activeSpawners.Add(FindObjectOfType<RoomSpawner>());

        RM = FindObjectOfType<RoomManager>();

        Invoke("Spawn", 0.01f);
    }

    private void Spawn()
    {
        //find last spawned Spawner
        int highestI = -1;
        int highestId = -1;

        for (int i =0; i < activeSpawners.Count; i++)
        {
            int currentId = activeSpawners[i].idSp;

            if (currentId > highestId)
            {
                highestI = i;
                highestId = currentId;
            }
        }

        if (highestI == -1)
        {
            RoomSpawner[] roomSpawners = FindObjectsOfType<RoomSpawner>();

            //spawn boss, chestRoom, shop
            SpawnRestOfRooms(roomSpawners);

            //delete all spawners
            allSpawned = true;

            Debug.Log("AllSpawned" + spawnedRooms + "/" + numOfRooms);

            //start RM
            RM.SetInfoOfAllRooms();

            foreach (RoomSpawner RS in roomSpawners)
            {
                Destroy(RS.gameObject);
            }

            return;
        }

        //if activeSpawners.Count + spawnedRooms < numOfRooms spawn only end Rooms, add new Spawners to list and remove used
        bool endSpawn = activeSpawners.Count + spawnedRooms < numOfRooms ? false : true;

        RoomSpawner[] newSpawners = activeSpawners[highestI].SpawnNewRoom(endSpawn);

        spawnedRooms++;

        activeSpawners.RemoveAt(highestI);

        foreach (RoomSpawner NS in newSpawners)
        {
            activeSpawners.Add(NS);
        }

        //create Loop
        Invoke("Spawn", 0.01f);        
    }

    private void SpawnRestOfRooms(RoomSpawner[] roomSpawners)
    {
        List<RoomSpawner> notSpawned = new List<RoomSpawner>();

        foreach (RoomSpawner RS in roomSpawners)
        {
            if (RS.transform.position.y >= dungeonHight)
            {
                notSpawned.Add(RS);
            }
        }

        if(notSpawned.Count <= 0) { return; }

        //specialRoomSpawn
        int randomBoss = Random.Range(0, notSpawned.Count -1);

        notSpawned[randomBoss].SpawnEnd(true);

        notSpawned.RemoveAt(randomBoss);

        //OtherSpawn
        foreach (RoomSpawner NS in notSpawned)
        {
            NS.SpawnEnd(false);
        }
    }
}
