using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    private GameObject Player;
    private Rigidbody2D rigidBody;
    private Collider2D colliderThis;
    public Animator bossAnimator;

    private bool isDoingAction = true;
    private float activeDoingActionTime = 0;

    public float waitingTime = 2;
    private float waitingActiveTime = 0;

    [Header("CollisionDamage")]
    public float collisionDamage = 10;
    public float collisionDamageInterval = 1.5f;
    private float actioveCollisionDamageInterval = 0;

    [Header("CircleAttack")]    
    public float circleAttackDuration = 5;
    public float projectileCircleAttackSpeed = 15;
    public float projectileCircleAttackDamage = 20;
    public float timeBetweenWaves = 0.5f;
    private float activeTimeBetweenWaves = 0;
    public float waitAfterCircleAttack = 5;
    public GameObject CircleAttackPrefab;  

    private bool attackingCircleShoot = false;
    private int random;

    [Header("SpawnAttack")]
    public float spawnAttackDuration = 2;
    public int numOfEnemiesSpawning = 2;
    public float shootingSpeed = 100;

    public GameObject NewEnemyPrefab;

    private bool attackingSpawnAttack = false;
    private int SpawnedSpawnAttack = 0;

    // Start is called before the first frame update
    void Start()
    {
        Player = FindObjectOfType<MainPlayerScript>().gameObject;
        rigidBody = GetComponent<Rigidbody2D>();
        colliderThis = GetComponent<Collider2D>();

        waitingActiveTime = 4;
    }

    // Update is called once per frame
    void Update()
    {
        //CountdownAction a Start Waiting BewtweenAttacks
        if (activeDoingActionTime > 0)
        {
            Debug.Log("DoingActuin");
            activeDoingActionTime -= Time.deltaTime;

            if (activeDoingActionTime < 0)
            {
                activeDoingActionTime = 0;
                bossAnimator.SetBool("Attacking", false);

                attackingCircleShoot = false;
                activeTimeBetweenWaves = 0;

                attackingSpawnAttack = false;
                SpawnedSpawnAttack = 0;

                waitingActiveTime += waitingTime;
            }
        }

        //CountdownBetweenActions WaitTime
        if (waitingActiveTime > 0)
        {
            Debug.Log("Waiting");
            waitingActiveTime -= Time.deltaTime;

            if (waitingActiveTime < 0)
            {
                waitingActiveTime = 0;
                isDoingAction = false;
            }
        }

        if(actioveCollisionDamageInterval > 0)
        {
            actioveCollisionDamageInterval -= Time.deltaTime;

            if (actioveCollisionDamageInterval < 0)
            {
                actioveCollisionDamageInterval = 0;
            }
        }

        //DifferentActions

        if (activeTimeBetweenWaves > 0)
        {
            activeTimeBetweenWaves -= Time.deltaTime;

            if (activeTimeBetweenWaves < 0)
            {
                activeTimeBetweenWaves = 0;
            }
        }

        if (attackingCircleShoot)
        {
            Debug.Log("cyrcle attack");
            if (activeTimeBetweenWaves <= 0 && activeDoingActionTime > waitAfterCircleAttack)
            {
                CircleAttack();
            }
        }

        if (attackingSpawnAttack)
        {
            Debug.Log("SpawnAttack");
            float SpawnRate = spawnAttackDuration / numOfEnemiesSpawning;

            if ((spawnAttackDuration - activeDoingActionTime) > SpawnRate * SpawnedSpawnAttack)
            {
                SpawnAttack();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDoingAction)
        {
            ChooseAction();
        }
    }

    private void ChooseAction()
    {
        int random = Random.Range(1, 3);

        switch (random)
        {
            case 1:
                StartCircleAttack();
                break;
            case 2:
                StartSpawnAttack();
                break;
        }

        isDoingAction = true;               
    }


    private void StartCircleAttack()
    {
        isDoingAction = true;
        attackingCircleShoot = true;
        bossAnimator.SetBool("Attacking", true);
        random = Random.Range(0, 2);
        activeDoingActionTime += circleAttackDuration + waitAfterCircleAttack;
    }

    private void CircleAttack()
    {
        activeTimeBetweenWaves += timeBetweenWaves;

        Vector2[] EveryDirection1 = new Vector2[] {new Vector2(0,1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1) };
        Vector2[] EveryDirection2 = new Vector2[] { new Vector2(0.5f, 1), new Vector2(1, 0.5f), new Vector2(1, -0.5f), new Vector2(0.5f, -1), new Vector2(-0.5f, -1), new Vector2(-1, -0.5f), new Vector2(-1, 0.5f), new Vector2(-0.5f, 1) };

        for (int i = 0; i < 8; i++)
        {
            GameObject newProjectile = Instantiate(CircleAttackPrefab, transform.position, Quaternion.identity);

            ProjectileBehavior ProjectileScript = newProjectile.GetComponent<ProjectileBehavior>();
            ProjectileScript.projectileDamage = projectileCircleAttackDamage;

            Rigidbody2D rigidBody = newProjectile.GetComponent<Rigidbody2D>();

            if (random == 0)
            {
                rigidBody.velocity = EveryDirection2[i].normalized * projectileCircleAttackSpeed;
            } else
            {
                rigidBody.velocity = EveryDirection1[i].normalized * projectileCircleAttackSpeed;
            }
        }
    }

    private void StartSpawnAttack()
    {
        int numOfEnemiesInRoom = gameObject.transform.parent.childCount;
        bossAnimator.SetBool("Attacking", true);

        if (numOfEnemiesInRoom >= 3)
        {
            StartCircleAttack();
        } else
        {
            isDoingAction = true;
            attackingSpawnAttack = true;
            activeDoingActionTime += spawnAttackDuration;

            if(numOfEnemiesInRoom == 2)
            {
                SpawnedSpawnAttack = 1;
                activeDoingActionTime -= spawnAttackDuration / 2;
            }
        }
    }

    private void SpawnAttack()
    {
        int randomDirx = Random.Range(-1, 2);
        Vector2 dir = new Vector2(randomDirx, 1).normalized;

        GameObject newEnemy = Instantiate(NewEnemyPrefab, new Vector3(transform.position.x, transform.position.y, NewEnemyPrefab.transform.position.z), Quaternion.identity,transform.parent);

        Collider2D colliderOfNewEnemy = newEnemy.GetComponent<Collider2D>();
        colliderOfNewEnemy.isTrigger = true;
        StartCoroutine(ChangeColliderOfNewEnemy(colliderOfNewEnemy));

        newEnemy.GetComponent<MainEnemyScript>().StunMe(3);

        newEnemy.GetComponent<Rigidbody2D>().velocity = dir * shootingSpeed;

        SpawnedSpawnAttack++;
    }
    
    private IEnumerator ChangeColliderOfNewEnemy(Collider2D collider)
    {
        yield return new WaitForSeconds(1);

        if (collider.isActiveAndEnabled)
        {
            collider.isTrigger = false;
        }
    }

    //CollisionDamage
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6 && actioveCollisionDamageInterval <= 0)
        {
            collision.gameObject.GetComponent<MainPlayerScript>().TakeDamage(collisionDamage);

            actioveCollisionDamageInterval += collisionDamageInterval;
        }
    }   
}
