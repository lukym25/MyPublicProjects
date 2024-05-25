using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    private EnemyMovement enemyMovement; 

    private EnemyHealth healthscript;

    // Start is called before the first frame update
    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        healthscript = GetComponent<EnemyHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyMovement.MoveOnPath();
    }
}
