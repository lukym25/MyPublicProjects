using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damageAmount = 50;
    public float attackRadius = 2;
    public float attackInterval = 1;
    public float projectileSpeed = 15;
    public float chargingAttackTime = 0.5f;
    public GameObject projectilePrefab;
    public Transform weaponHitbox;
    public LayerMask Players;
    public LayerMask enemies;
    public LayerMask destructableObjects;

    public Animator enemyAnimator;

    public float Attack(bool isMelee, Vector2 playerPos)
    {
        LookToPlayer(playerPos);

        if (isMelee)
        {
            StartCoroutine(Swing());
        }
        else
        {
            StartCoroutine(Shoot(playerPos));
            PlayShootAnim(chargingAttackTime - 0.3f);
        }

        return attackInterval + chargingAttackTime;        
    }

    IEnumerator PlayShootAnim(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        enemyAnimator.SetTrigger("Attack");
    }


    IEnumerator Swing()
    {
        yield return new WaitForSeconds(chargingAttackTime);

        Debug.Log("swing");

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(weaponHitbox.position, attackRadius, Players);
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(weaponHitbox.position, attackRadius, destructableObjects);

        foreach (Collider2D HPl in hitPlayers)
        {
            HPl.gameObject.GetComponent<MainPlayerScript>().TakeDamage(damageAmount);
        }

        foreach (Collider2D HO in hitObjects)
        {
            HO.GetComponent<DestrucatableObject>().DestroyMe();
        }
    }


    IEnumerator Shoot(Vector2 playerPos)
    {
        yield return new WaitForSeconds(chargingAttackTime);

        Vector2 dir = (playerPos - (Vector2)weaponHitbox.position).normalized;

        GameObject newObject = Instantiate(projectilePrefab, weaponHitbox.position, Quaternion.identity);

        newObject.GetComponent<Rigidbody2D>().velocity = dir * projectileSpeed;

        newObject.GetComponent<ProjectileBehavior>().projectileDamage = damageAmount;
    }

    private void LookToPlayer(Vector2 playerPos)
    {
        Vector2 dir = (playerPos - (Vector2)transform.position).normalized;

        if (dir.x > 0)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (dir.x < 0)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (weaponHitbox == null) { return; }

        Gizmos.DrawWireSphere(weaponHitbox.position, attackRadius);
    }
    
}