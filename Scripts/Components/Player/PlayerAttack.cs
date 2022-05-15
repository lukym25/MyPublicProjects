using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private MainPlayerScript MPS;

    [SerializeField]
    private LayerMask enemies, destructableObjects;
    [SerializeField]
    private Transform fistHittbox;
    
    public Animator swordAnimator;

    [HideInInspector]
    public bool flipped = false;
    private float activeCoolDown;

    //weaponInfo
    [SerializeField]
    private float coolDown, attackRange, weaponDamage, knockBack;

    //??
    public Vector2 beanColider;
    public float startingAngle = 0;
    //??
    private Vector2 lookAt;
    private Vector2 lookDir;

    //??
    private Joystick joystick;


    // Start is called before the first frame update
    private void Start()
    {
        MPS = GetComponent<MainPlayerScript>();

        if (Settings.mobileMode)
        {
            joystick = FindObjectOfType<Joystick>();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (activeCoolDown > 0)
        {
            activeCoolDown -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if (activeCoolDown <= 0)
        {
            Debug.Log("Attack");

            activeCoolDown = coolDown;

            swordAnimator.SetTrigger("Attack");

            CheckZone();
        }
    }

    private void CheckZone() 
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(fistHittbox.position, attackRange, enemies);
        
        foreach (Collider2D HE in hitEnemies)
        {
            HE.gameObject.GetComponent<MainEnemyScript>().TakeDamage(weaponDamage);

            KnockBackEffect(HE.gameObject);
        }
        
        Collider2D[] hitDestructableObjects = Physics2D.OverlapCircleAll(fistHittbox.position, attackRange, destructableObjects);

        foreach (Collider2D HO in hitDestructableObjects)
        {
            HO.GetComponent<DestrucatableObject>().DestroyMe();
        }
    }

    private void KnockBackEffect(GameObject HE)
    {
        //knockback
        Rigidbody2D rigidBoryHitObject = HE.GetComponent<Rigidbody2D>();

        float dirOfKnockBackX = (transform.position.x - HE.transform.position.x) > 0 ? -1 : 1;

        Vector2 dirOfKnockBack = new Vector2(dirOfKnockBackX, 2f).normalized;

        Debug.Log(dirOfKnockBack);

        rigidBoryHitObject.AddForce(dirOfKnockBack * knockBack, ForceMode2D.Impulse);
    }


    public void FlipWeapon(bool normal)
    {
        /*if (normal)
        {
            fistHittbox.localScale = new Vector3(1, fistHittbox.localScale.y, fistHittbox.localScale.z);
        }
        else
        {
            fistHittbox.localScale = new Vector3(-1, fistHittbox.localScale.y, fistHittbox.localScale.z);
        }*/
    }

    private void OnDrawGizmosSelected()
    {
        if (fistHittbox == null) { return; }

        Gizmos.DrawWireSphere(fistHittbox.position, attackRange);
    }
}
