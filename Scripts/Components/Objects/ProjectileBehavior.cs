using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float projectileDamage = 10;
    public int numOfBounces = 0;
    public LayerMask targetLayer;
    public LayerMask destructableObjects;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == Mathf.Log(targetLayer.value, 2))
        {
            if (collision.gameObject.layer == 6)
            {
                collision.gameObject.GetComponent<MainPlayerScript>().TakeDamage(projectileDamage);
                Destroy(gameObject);
            } else if (collision.gameObject.layer == 7)
            {
                collision.gameObject.GetComponent<MainEnemyScript>().TakeDamage(projectileDamage);
                Destroy(gameObject);
            }          
        }

        else if (collision.gameObject.layer == Mathf.Log(destructableObjects.value, 2))
        {
            collision.gameObject.GetComponent<DestrucatableObject>().DestroyMe();
        }
        else if (collision.gameObject.tag == "HalfFloor")
        {

        }
        else if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
        {
            /*if (collision.gameObject.name.Remove(4) == "Boss" )
            {
                if (numOfBounces <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Bounce(collision);
                }
            }*/
        }
        else if (collision.gameObject.tag == "Projectile")
        {
            ProjectileBehavior ProjectleScriptOtherObject = collision.gameObject.GetComponent<ProjectileBehavior>();

            if (!(ProjectleScriptOtherObject.targetLayer == targetLayer))
            {

                //Show explosion Effecxt
                //Destroy(gameObject);
            }
        }
        else
        {
            if (numOfBounces <= 0)
            {
                Destroy(gameObject);
            } else
            {
                Bounce(collision);
            }
        }
    }

    private void Bounce(Collider2D collision)
    {
        Vector2 contactPoint = collision.ClosestPoint(transform.position);

        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();

        Vector2 velocity = rigidBody.velocity;

        Vector2 dirFromProjectileToContact = contactPoint - (Vector2)transform.position;

        if (Mathf.Abs(dirFromProjectileToContact.x) > Mathf.Abs(dirFromProjectileToContact.y))
        {
            rigidBody.velocity = new Vector2(velocity.x * (-1), velocity.y);
        }
        else
        {
            rigidBody.velocity = new Vector2(velocity.x, velocity.y * (-1));
        }

        numOfBounces--;
    }
}
