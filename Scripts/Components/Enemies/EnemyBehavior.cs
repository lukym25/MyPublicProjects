using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyBehavior : MonoBehaviour
{
    //movement
    public float walkSpeed = 5;
    public float chaseSpeed = 10;
    public float acceleration = 5;
    public float decceleration = 5;
    public float jumpForce = 30;
    public float frictionAmount = 0.2f;
    public bool isFlying = false;
    public float jumpInterval = 2;

    private float jumpCooldown = 0;

    //Pathfinding
    public float waypointDistance = 3;

    private Path path;
    private Seeker seeker;
    private int currentWaypoint = 0;

    //action
    public float chaseDistance = 0;
    public float attackDistance = 5;
    public bool isMelee = false;
    public bool stopWhileAttacking = true;
    private float timeToAttack = 0;

    private Rigidbody2D RB;
    private EnemyAttack EAS;
    private GameObject[] players;
    private Vector2 nearestPlayerPos;

    private float currentDistance = 0;

    //effects
    public float stunTime = 0;


    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        EAS = GetComponent<EnemyAttack>();
        seeker = GetComponent<Seeker>();

        CalculateNearestPlayer();

        InvokeRepeating("UpdatePath", 0f, 1f);

        if (isFlying) { RB.gravityScale = 0; }
    }

    private void Update()
    {
        CalculateNearestPlayer();

        if (timeToAttack > 0)
        {
            timeToAttack -= Time.deltaTime;
        }

        if (jumpCooldown > 0)
        {
            jumpCooldown -= Time.deltaTime;
        }

        if (stunTime > 0)
        {
            stunTime -= Time.deltaTime;
        }
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(RB.position, nearestPlayerPos, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    private void CalculateNearestPlayer()
    {
        int index = -1;
        float distance = -1;

        for (int i = 0; i < players.Length; i++)
        {
            currentDistance = Vector2.Distance(gameObject.transform.position, players[i].gameObject.transform.position);

            if (currentDistance > distance)
            {
                index = i;
            }
        }

        if (index != -1)
        {
            nearestPlayerPos = (Vector2)players[index].transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (nearestPlayerPos != null && stunTime <= 0)
        {
            bool seePlayer = SeePlayer();

            if (currentDistance < attackDistance && seePlayer && timeToAttack <= 0)
            {
                Attack();
            }
            else if (currentDistance < chaseDistance && seePlayer && timeToAttack <= 0)
            {
                Chase();
            }
            else
            {
                if (timeToAttack <= 0 || isMelee)
                {
                    Roam();
                }
            }
        }
    }
    private bool SeePlayer()
    {
        Vector2 start = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

        Vector2 end = new Vector2(nearestPlayerPos.x, nearestPlayerPos.y);

        RaycastHit2D hit = Physics2D.Linecast(start, end, 1 << 10);

        bool seePlayer;

        if (hit.collider != null)
        {
            seePlayer = hit.collider.gameObject.name.Remove(4) == "Half" ? true : false;

        }
        else
        {
            seePlayer = true;
        }

        Debug.DrawLine(start, end, Color.green);

        Debug.Log(name + "das" + seePlayer);

        return seePlayer;
    }

    private void Attack()
    {
        //stop player (friction)
        float frictionForce = Mathf.Min(Mathf.Abs(RB.velocity.x), frictionAmount) * Mathf.Sign(RB.velocity.x);
        RB.AddForce(Vector2.right * -frictionForce, ForceMode2D.Impulse);

        timeToAttack = EAS.Attack(isMelee, nearestPlayerPos);
    }

    private void Roam()
    {       
        if (currentDistance < chaseDistance)
        {
            Move(walkSpeed, true);
        }
        else
        {
            Move(walkSpeed, false);
        }
    }

    private void Chase()
    {
        Move(chaseSpeed, true);
    }

    private void Move(float speed, bool chase)
    {
        Debug.Log("Roam" + name);

        if (path == null) { return; }

        float distance = Vector2.Distance(RB.position, path.vectorPath[currentWaypoint]);

        if (distance < waypointDistance)
        {
            if (currentWaypoint < path.vectorPath.Count - 1)
            {
                currentWaypoint++;
            }
        }

        //calculating acceleration force and dir 
        Vector2 movement;

        Vector2 destinydir;

        if (chase)
        {
            destinydir = nearestPlayerPos - (Vector2)transform.position;
        } else
        {
            destinydir = path.vectorPath[currentWaypoint] - transform.position;
        }

        if (isFlying)
        {
            Vector2 targetSpeed = destinydir.normalized * speed;

            Vector2 speedDif = targetSpeed - RB.velocity;

            movement = speedDif * acceleration;
        } else
        {
            float targetSpeed = Mathf.Sign(destinydir.x) * speed;

            float speedDif = targetSpeed - RB.velocity.x;

            float movementX = Mathf.Abs(speedDif) * acceleration * Mathf.Sign(speedDif);

            movement = Vector2.right * movementX;
        }


        if (!isFlying && Mathf.Abs(destinydir.normalized.x) < 0.1 && destinydir.y > 0)
        {
            Jump();
            return;
        }
        Debug.Log("Move" + name + movement + RB.velocity + "BB");

        RB.AddForce(movement);

        Debug.Log("Move" + name + movement + RB.velocity + "AA");

        //jump
        if (destinydir.y >= Mathf.Abs(destinydir.x) && Mathf.Abs(destinydir.x) < 5 && !isFlying)
        {
            Jump();
        }

        //sprite Orientation
        if (movement.x > 0.2)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (movement.x < -0.2)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Jump()
    {
        if(jumpCooldown <= 0)
        {
            RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpCooldown = jumpInterval;
        }
    }

}
