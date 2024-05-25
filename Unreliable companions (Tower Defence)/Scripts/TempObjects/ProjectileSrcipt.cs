using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSrcipt : MonoBehaviour
{
    [SerializeField]
    private bool focusEnemy = true;

    public float damage = 0;
    public float speed = 0;
    public float remainingRange = 0;


    private void Update()
    {
        CheckRange();
    }

    private void CheckRange()
    {
        remainingRange -= speed * Time.deltaTime;
        if (remainingRange <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (focusEnemy)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                EnemyHit(collision);
            }    
            
            Destroy(gameObject);       
        }
        else 
        {
            if (collision.gameObject.tag == "Player") 
            {
                Playerhit(collision);
            }
            
            Destroy(gameObject);            
        }        
    }

    private void EnemyHit(Collision2D collision)
    {
        collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
    }

    private void Playerhit(Collision2D collision)
    {

    }
}


