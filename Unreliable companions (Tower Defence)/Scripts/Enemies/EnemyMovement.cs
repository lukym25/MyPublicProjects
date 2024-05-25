using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    private Rigidbody2D rigidBody;
    public Vector2[] waypoints;

    private int currentWaypoint = 0;
    private bool onEndOfPath = false;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();   
    }

    public void MoveOnPath()
    {
        if(onEndOfPath) { return; }

        if (Vector2.Distance(transform.position, waypoints[currentWaypoint]) < 0.5f)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                onEndOfPath = true;
                return;
            }
        }

        Vector2 dir = waypoints[currentWaypoint] - (Vector2)transform.position;

        rigidBody.velocity = dir.normalized * movementSpeed;
    }
}
