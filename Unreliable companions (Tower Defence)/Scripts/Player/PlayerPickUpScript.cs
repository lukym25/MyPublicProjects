using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpScript : MonoBehaviour
{
    private ShopScript shopScript;

    [SerializeField]
    private float pickUpRange, pickUpSpeed;

    private void Start()
    {
        shopScript = GameObject.FindGameObjectWithTag("GM").GetComponent<ShopScript>();
    }


    // Update is called once per frame
    void Update()
    {
        MoveItemsTowardsPlayer();
    }

    private void MoveItemsTowardsPlayer()
    {
        GameObject[] collectables = GameObject.FindGameObjectsWithTag("Collectable");

        foreach (GameObject item in collectables)
        {
            float distance = Vector2.Distance(transform.position, item.transform.position);

            if (distance > pickUpRange) 
            {
                item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            } else
            {
                Vector2 direction = transform.position - item.transform.position;
                Vector2 speed = direction.normalized * pickUpRange / distance * pickUpSpeed;

                Debug.Log(speed);

                item.GetComponent<Rigidbody2D>().velocity = speed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Collectable")
        {
            shopScript.CoinsValueChanged(collision.gameObject.GetComponent<CoinScript>().coinValue);

            Destroy(collision.gameObject);
        }
    }
}
