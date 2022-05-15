using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomSpawner : MonoBehaviour
{
    public string dir;
    private MainMapLoader MML;
    [HideInInspector]
    public static int idNumAll = 0;
    public int idSp = 0;
    private GameObject newRoom;
    private GameObject[] rooms;

    // Start is called before the first frame update
    void Start()
    {
        MML = FindObjectOfType<MainMapLoader>();

        idNumAll++;

        idSp = idNumAll;
    }

    public RoomSpawner[] SpawnNewRoom(bool spawnEnd)
    {
        if (MML.spawnedRooms == 0)
        {
            rooms = MML.startingRoom;
        }
        //too high        
        else if (transform.position.y >= MML.dungeonHight)
        {
            FillEnd();
        }
        //too far on y
        else if (Mathf.Abs(transform.position.x) >= MML.dungeonWidth)
        {
            FillEnd();
        }
        else if (spawnEnd)
        {
            FillEnd();
        } else
        {
            bool colisionY;
            bool colisionX;

            switch (dir)
            {
                case "U":
                    colisionY = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, 40), 1) != null ? true : false;
                    colisionX = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(70.2f, 0), 1) != null ? true : false;
                    bool colisionX1 = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(-70.2f, 0), 1) != null ? true : false;

                    //if colide
                    if (colisionY || colisionX || colisionX1)
                    {
                        if (colisionY)
                        {
                            rooms = MML.sideWayEnd;
                        }
                        else if (colisionX && colisionX1)
                        {
                            rooms = MML.sideWayEnd.Concat(MML.upRoomsI).ToArray();
                        }
                        else if(colisionX)
                        {
                            rooms = MML.sideWayEnd.Concat(MML.upRoomsI).Concat(MML.upRoomsTL).ToArray();
                        } else
                        {
                            rooms = MML.sideWayEnd.Concat(MML.upRoomsI).Concat(MML.upRoomsTR).ToArray();
                        }
                    }
                    //if to few rooms
                    else if (!spawnEnd)
                    {
                        rooms = MML.upRoomsX.Concat(MML.upRoomsTR).Concat(MML.upRoomsTL).Concat(MML.upRoomsI).ToArray();
                    }
                    else
                    {
                        rooms = MML.sideWayEnd.Concat(MML.upRoomsX).Concat(MML.upRoomsTR).Concat(MML.upRoomsTL).Concat(MML.upRoomsI).ToArray(); ;
                    }
                    break;
                case "R":
                    colisionY = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, 40), 1) != null ? true : false;
                    colisionX = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(-70.2f, 0), 1) != null ? true : false;

                    //if colide
                    if (colisionY || colisionX)
                    {
                        if (colisionY && colisionX)
                        {
                            rooms = MML.rightRoomsEnd;
                        } else if(colisionY)
                        {
                            rooms = MML.rightRoomsEnd.Concat(MML.rightRoomsI).ToArray();
                        }
                        else 
                        {
                            rooms = MML.rightRoomsEnd.Concat(MML.rightRoomsL).ToArray();
                        }
                    } 
                    //if to few rooms
                    else if (!spawnEnd)
                    {
                        rooms = MML.rightRoomsT.Concat(MML.rightRoomsI).Concat(MML.rightRoomsL).ToArray();
                    }
                    else
                    {
                        rooms = MML.rightRoomsEnd.Concat(MML.rightRoomsT).Concat(MML.rightRoomsI).Concat(MML.rightRoomsL).ToArray();
                    }
                    break;
                case "L":
                    colisionY = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, 40), 1) != null ? true : false;
                    colisionX = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(70.2f, 0), 1) != null ? true : false;

                    //if colide
                    if (colisionY || colisionX)
                    {

                        if (colisionY && colisionX)
                        {
                            rooms = MML.leftRoomsEnd;
                        }
                        else if (colisionY)
                        {
                            rooms = MML.leftRoomsEnd.Concat(MML.leftRoomsI).ToArray();
                        }
                        else
                        {
                            rooms = MML.leftRoomsEnd.Concat(MML.leftRoomsL).ToArray();
                        }
                    }
                    //if to few rooms
                    else if (!spawnEnd)
                    {
                        rooms = MML.leftRoomsT.Concat(MML.leftRoomsI).Concat(MML.leftRoomsL).ToArray();
                    }
                    else
                    {
                        rooms = MML.leftRoomsEnd.Concat(MML.leftRoomsT).Concat(MML.leftRoomsI).Concat(MML.leftRoomsL).ToArray();
                    }
                    break;
                default:
                    rooms = new GameObject[0];
                    break;
            }
        }

        if (rooms.Length <= 0)
        {
            return new RoomSpawner[0];
        }

        CreteRoomObject();

        RoomSpawner[] newSpawners = newRoom.GetComponentsInChildren<RoomSpawner>();

        return newSpawners;
    }

    public void SpawnEnd(bool spawnBoss)
    {
        if (spawnBoss)
        {
            rooms = MML.bossRoom;
        } else
        {
            rooms = MML.sideWayEnd;
        }

        CreteRoomObject();
    }

    private void FillEnd()
    {
        switch (dir)
        {
            case "U":
                rooms = new GameObject[0];
                break;
            case "R":
                rooms = MML.rightRoomsEnd;
                break;
            case "L":
                rooms = MML.leftRoomsEnd;
                break;
            default:
                rooms = new GameObject[0];
                break;
        }
    }

    private void CreteRoomObject()
    {
        float random = Random.Range(0, 100);

        for (int i = 0; i < rooms.Length; i++)
        {
            if (random < (i + 1) * 100 / rooms.Length)
            {
                newRoom = Instantiate(rooms[i], transform.position, Quaternion.identity, transform.parent.parent.transform);

                break;
            }
        }
    }
}

