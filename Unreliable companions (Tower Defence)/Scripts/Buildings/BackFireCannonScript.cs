using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackFireCannonScript : MonoBehaviour
{
    private BuildingInfo buildingInfo;

    [SerializeField]
    private GameObject rotateObject;

    [SerializeField]
    private Transform MainFirePoint, SecondFirePoint, ThirdFirePoint;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private int cyklusCount;

    [SerializeField]
    private GameObject projectilePrefabOnPlayer, projectilePrefabOnEnemy;

    private typesOfTurretFocus focusMode = typesOfTurretFocus.First;

    private float activeCoolDown = 0.5f;

    private int cyklus = 0;

    private bool readyToAttack = false;

    private void Awake()
    {
        buildingInfo = GetComponent<BuildingInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        CoolDownTick();

        RotateTowardsEnemy();

        Attack();
    }

    private void CoolDownTick()
    {
        if (activeCoolDown <= 0) { return; }

        activeCoolDown -= Time.deltaTime;

        if(activeCoolDown < 0) { activeCoolDown = 0; }        
    }

    private void RotateTowardsEnemy()
    {
        GameObject[] enemiesInRange = FindAllEnemiesInRange();

        if(enemiesInRange.Length == 0) { readyToAttack = false; return; }

        GameObject focusedEnemy = enemiesInRange[0];

        switch (focusMode) 
        {
            case typesOfTurretFocus.First:

                break;

            case typesOfTurretFocus.Last:

                break;
            case typesOfTurretFocus.LowHp:

                break;
            case typesOfTurretFocus.HighHp:

                break;
        }


        Vector3 vectorToTarget = focusedEnemy.transform.position - transform.position;

        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;

        Debug.Log(angle + "/" + (rotateObject.transform.rotation.z * Mathf.Rad2Deg));

        Vector2 difference = rotateObject.transform.up - vectorToTarget.normalized;

        Debug.Log(difference);

        readyToAttack = Mathf.Abs(difference.x) < 0.02f && Mathf.Abs(difference.y) < 0.02f ? true: false;

        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

        rotateObject.transform.rotation = Quaternion.Slerp(rotateObject.transform.rotation, q, Time.deltaTime * rotationSpeed);
    }

    private GameObject[] FindAllEnemiesInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        List<int> enemiesInRangeId = new List<int>();

        for(int i = 0; i < enemies.Length; i++)
        {
            if(Vector2.Distance(transform.position, enemies[i].transform.position) <= buildingInfo.range)
            {
                enemiesInRangeId.Add(i);
            }
        }

        GameObject[] enemiesInRange = new GameObject[enemiesInRangeId.Count];

        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            enemiesInRange[i] = enemies[enemiesInRangeId[i]];
        }

        return enemiesInRange;
    }
    

    private void Attack()
    {
        if (activeCoolDown != 0) { return; }
        if (!readyToAttack) { return; }
        if(CoreHpScript.instance.gameOver) { return; }

        FireProjectile(MainFirePoint, projectilePrefabOnEnemy);

        cyklus++;
        if (cyklus >= cyklusCount)
        {
            FireProjectile(SecondFirePoint, projectilePrefabOnPlayer);
            FireProjectile(ThirdFirePoint, projectilePrefabOnPlayer);

            cyklus = 0;
        }

        SoundManager.instance.PlaySound("TurretShoot");

        activeCoolDown += 1/ buildingInfo.attackSpeed;
    }

    private void FireProjectile(Transform firePoint, GameObject prefab)
    {
        GameObject newProjectile = Instantiate(prefab, firePoint.position, firePoint.rotation);

        ProjectileSrcipt projectileScript = newProjectile.GetComponent<ProjectileSrcipt>();

        projectileScript.damage = buildingInfo.damage;
        projectileScript.speed = buildingInfo.projectileSpeed;
        projectileScript.remainingRange = buildingInfo.range;

        newProjectile.GetComponent<Rigidbody2D>().velocity = firePoint.up * buildingInfo.projectileSpeed;
    }


    private enum typesOfTurretFocus
    {
        First, Last, LowHp, HighHp
    }
}

