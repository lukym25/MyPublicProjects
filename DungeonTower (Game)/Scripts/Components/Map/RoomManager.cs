using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public float timeOfTranlsation = 1;
    public Transform cameraPos;
    public GameObject activeRoom;

    private GameObject[] allRooms;
    private float firstDistance;
    private bool movingCam = false;
    private GameObject Player;
    private bool doorsDown = false;
    private GridGraph gg;

    private GameObject BossSlider;

    private void FixedUpdate()
    {
        if (movingCam)
        {
            float currentDistance = Vector2.Distance(cameraPos.position, activeRoom.transform.position);

            float MathFunc = Mathf.Sin((1 - currentDistance / firstDistance) - Mathf.PI) + 1;

            float speed = MathFunc / timeOfTranlsation;

            cameraPos.position = Vector3.MoveTowards(cameraPos.position, activeRoom.transform.position + new Vector3(0, 0, -10), speed);

            if (currentDistance <= 0)
            {
                movingCam = false;
            }
        }
        if (doorsDown)
        {
            if (activeRoom.transform.GetChild(2).childCount <= 0)
            {
                DoorsUp();

                if (activeRoom.name.Remove(4) == "Boss")
                {
                    SceneManager.LoadScene(0);
                }
            }
        }
    }

    public void SetInfoOfAllRooms()
    {
        Player = FindObjectOfType<MainPlayerScript>().gameObject;

        allRooms = GameObject.FindGameObjectsWithTag("Room");

        foreach (GameObject R in allRooms)
        {
            if ((Vector2)R.transform.position == Vector2.zero)
            {
                activeRoom = R;

                //activating scripts in current room to jump to other room
                activeRoom.transform.GetChild(0).gameObject.SetActive(true);

                //activating monster in current room
                activeRoom.transform.GetChild(2).gameObject.SetActive(true);
            }
        }

        AstarData aData = AstarPath.active.data;

        gg = aData.gridGraph;

        AstarPath.active.Scan();
    }

    public void SwitchActiveRoom(Vector2 newScenePos)
    {
        foreach(GameObject R in allRooms)
        {
            if((Vector2)R.transform.position == newScenePos)
            {
                //disabiling scripts in current room to jump to other room
                activeRoom.transform.GetChild(0).gameObject.SetActive(false);

                //disabiling monster in current room
                activeRoom.transform.GetChild(2).gameObject.SetActive(false);

                firstDistance = Vector2.Distance(activeRoom.transform.position, newScenePos);

                if (doorsDown)
                {
                    DoorsUp();
                }

                activeRoom = R;

                gg.center = newScenePos;

                AstarPath.active.Scan();

                //activating scripts in current room to jump to other room
                activeRoom.transform.GetChild(0).gameObject.SetActive(true);

                //activating monster in current room
                activeRoom.transform.GetChild(2).gameObject.SetActive(true);

                if(activeRoom.transform.GetChild(2).childCount > 0)
                {
                    DoorsDown();
                } 

                movingCam = true;

                if (activeRoom.name.Remove(4) == "Boss")
                {
                    BossRoomActive();
                }
            }
        }
    }

    private void DoorsDown()
    {
        doorsDown = true;

        activeRoom.transform.GetChild(1).gameObject.SetActive(true);
    }
    
    private void DoorsUp()
    {
        doorsDown = false;

        activeRoom.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void BossRoomActive()
    {
        FindObjectOfType<UIInGame>().ShowBossSlider();
    }
}
