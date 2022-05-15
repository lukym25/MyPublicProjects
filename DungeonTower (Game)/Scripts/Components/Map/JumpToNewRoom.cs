using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToNewRoom : MonoBehaviour
{
    public string dir;
    private RoomManager RM;

    private void Start()
    {
        RM = FindObjectOfType<RoomManager>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6 && gameObject.activeInHierarchy)
        {
            Vector2 newPos;
            Vector2 playerPos = collision.gameObject.transform.position;

            switch (dir) 
            {
                case "U":
                    if(playerPos.y > transform.position.y)
                    {
                        newPos = (Vector2)transform.parent.position + new Vector2(0, 40);

                        RM.SwitchActiveRoom(newPos);
                    }
                    break;
                case "R":
                    if (playerPos.x > transform.position.x)
                    {
                        newPos = (Vector2)transform.parent.position + new Vector2(70.2f, 0);

                        RM.SwitchActiveRoom(newPos);
                    }
                    break;
                case "D":
                    if (playerPos.y < transform.position.y)
                    {
                        newPos = (Vector2)transform.parent.position + new Vector2(0, -40);

                        RM.SwitchActiveRoom(newPos);
                    }
                    break;
                case "L":
                    if (playerPos.x < transform.position.x)
                    {
                        newPos = (Vector2)transform.parent.position + new Vector2(-70.2f, 0);

                        RM.SwitchActiveRoom(newPos);
                    }
                    break;
                default:                
                    break;
            }
        }
    }
}
