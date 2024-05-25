using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    private ShopScript shopScript;

    [SerializeField]
    private float pickUpRange, pickUpSpeed;

    private void Start()
    {
        shopScript = GetComponent<ShopScript>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveItemsTowardsPlayer();
    }

    private void MoveItemsTowardsPlayer()
    {
        GameObject[] collectables = GameObject.FindGameObjectsWithTag("Collectable");

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        foreach (GameObject item in collectables)
        {
            float distance = Vector2.Distance(mousePos, item.transform.position);
            if (distance < 0.5f)
            {
                ContactWithMouse(item);
            }
            else if (distance > pickUpRange)
            {
                item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            else
            {
                Vector2 direction = mousePos - (Vector2)item.transform.position;
                Vector2 speed = direction.normalized * pickUpRange / distance * pickUpSpeed;

                item.GetComponent<Rigidbody2D>().velocity = speed;
            }
        }
    }

    private void ContactWithMouse(GameObject collectableObject)
    {
        shopScript.CoinsValueChanged(collectableObject.GetComponent<CoinScript>().coinValue);

        SoundManager.instance.PlaySound("CoinPickUp");

        Destroy(collectableObject.gameObject);        
    } 
}
