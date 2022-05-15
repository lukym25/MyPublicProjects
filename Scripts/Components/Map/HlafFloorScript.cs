using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HlafFloorScript : MonoBehaviour
{
    private Collider2D myColider;
    private Transform Player;

    private void Start()
    {
        myColider = GetComponent<Collider2D>();
        Player = FindObjectOfType<MainPlayerScript>().gameObject.transform;
    }

    public void Disable()
    {
        myColider.isTrigger = true;
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            myColider.isTrigger = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Player.position.y < transform.position.y)
        {
            myColider.isTrigger = true;
        }
        else
        {
            myColider.isTrigger = false;
        }
    }
}
