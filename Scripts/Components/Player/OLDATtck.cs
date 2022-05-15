using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackOLD : MonoBehaviour
{
    private MainPlayerScript MPS;

    public LayerMask enemies;
    public LayerMask destructableObjects;
    public Transform fistHittbox;
    public Vector2 beanColider;
    public float startingAngle = 0;

    public Weapon cuuretWeapon;
    public UpgradesOnPlayer weaponUpgrades;

    [HideInInspector]
    public bool flipped = false;
    private float coolDown;
    private Vector2 lookAt;
    private Vector2 lookDir;

    private Joystick joystick;

    private void Start()
    {
        MPS = GetComponent<MainPlayerScript>();

        if (Settings.mobileMode)
        {
            joystick = FindObjectOfType<Joystick>();
        }
    }

    private void LateUpdate()
    {
        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }

        UpdatePosHittBox();
    }

    public void Attack()
    {
        if (coolDown <= 0)
        {
            Debug.Log("Attack");

            if (cuuretWeapon.melee)
            {
                Swing();
            } else
            {
                Shoot();
            }

            coolDown = 1 / cuuretWeapon.attackSpeed + weaponUpgrades.attackSpeedUpgrade;

            MPS.animator.SetTrigger("Attack");
        }
    }

    private void Swing()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(fistHittbox.position, cuuretWeapon.attackRange + weaponUpgrades.rangeUpgrade, enemies);
        Collider2D[] hitobjects = Physics2D.OverlapCircleAll(fistHittbox.position, cuuretWeapon.attackRange + weaponUpgrades.rangeUpgrade, destructableObjects);

        foreach (Collider2D HE in hitEnemies)
        {
            HE.gameObject.GetComponent<MainEnemyScript>().TakeDamage(cuuretWeapon.damage + weaponUpgrades.damageUpgrade);
        }
        foreach (Collider2D HO in hitobjects)
        {
            HO.GetComponent<DestrucatableObject>().DestroyMe();
        }
    }

    private void Shoot()
    {
        GameObject newPorjctile = Instantiate(cuuretWeapon.Projectile, fistHittbox.position, Quaternion.identity);

        newPorjctile.GetComponent<Rigidbody2D>().velocity = lookDir.normalized * cuuretWeapon.projectileSpeed;

        newPorjctile.GetComponent<ProjectileBehavior>().projectileDamage = cuuretWeapon.damage;
    }

    public void ChooseWeapon(Weapon newWeapon)
    {
        cuuretWeapon = newWeapon;
    }

    private void UpdatePosHittBox() 
    {
        if(!Settings.mobileMode)
        {
            lookAt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        } else
        {
            MainEnemyScript[] enemies = FindObjectsOfType<MainEnemyScript>();

            int maxI = -1;

            float maxDistance = -1;

            for (int i = 0; i < enemies.Length; i++)
            {
                float distance = Vector2.Distance(enemies[i].gameObject.transform.position, transform.position);
                if (distance < maxDistance || maxDistance == -1)
                {
                    maxI = i;
                    maxDistance = distance;
                }
            }
            
            if(maxI != -1)
            {
                lookAt = enemies[maxI].gameObject.transform.position;
            } else
            {
                if (joystick.Horizontal == 0 && joystick.Vertical == 0) { return; }

                lookAt = transform.position + new Vector3(joystick.Horizontal, joystick.Vertical, 0);
            }
        }

        lookDir = (lookAt - (Vector2)transform.position).normalized;

        fistHittbox.position = new Vector2(lookDir.x * (beanColider.x + 1), lookDir.y * (beanColider.y + 1)) + (Vector2)transform.position;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - startingAngle;

        if (Mathf.Abs(angle) < 90)
        {
            fistHittbox.localScale = new Vector3(fistHittbox.localScale.x, 1, fistHittbox.localScale.z);
        }
        else
        {
            fistHittbox.localScale = new Vector3(fistHittbox.localScale.x, -1, fistHittbox.localScale.z);
        }

        fistHittbox.eulerAngles = new Vector3(0, 0, angle);
        
    }

    public void FlipWeapon(bool normal)
    {
        if (normal)
        {
            fistHittbox.localScale = new Vector3(1, fistHittbox.localScale.y, fistHittbox.localScale.z);
        }
        else
        {
            fistHittbox.localScale = new Vector3(-1, fistHittbox.localScale.y, fistHittbox.localScale.z);
        }
    }
    /*
    private void OnDrawGizmosSelected()
    {
        if(fistHittbox == null) { return; }

        Gizmos.DrawWireSphere(fistHittbox.position, attackRadius);
    }
    */
}
