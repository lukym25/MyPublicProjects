using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour 
{
    [SerializeField]
    private float maxHp;

    [Header("Coins")]
    [SerializeField]
    private int dropedCoinsOnDeath;

    [SerializeField]
    private GameObject goldCoinPrefab, silverCoinPrefab, copperCoinPrefab;

    [SerializeField]
    private Vector3 maxStartingSpeed;

    private float currentHp;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float amount)
    {
        currentHp -= amount;

        if(currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        DropCoins();

        SoundManager.instance.PlaySound("EnemyDied");

        Destroy(gameObject);
    }

    private void DropCoins()
    {
        dropedCoinsOnDeath = (int)(dropedCoinsOnDeath * Random.Range(0.75f, 1.25f));

        int goldCoins = dropedCoinsOnDeath / 10;
        int silverCoins = (dropedCoinsOnDeath % 10) / 5;
        int copperCoins = (dropedCoinsOnDeath % 10) % 5;

        for (int i = 0; i < goldCoins + silverCoins + copperCoins; i++)
        {
            if (i < goldCoins)
            {
                InstantiateCoin(goldCoinPrefab, 10);
            } 
            else if (i < goldCoins + silverCoins)
            {
                InstantiateCoin(silverCoinPrefab, 5);
            }
            else 
            {
                InstantiateCoin(copperCoinPrefab, 1);
            }
        }
    }

    private void InstantiateCoin(GameObject coinPrefab, int value)
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, coinPrefab.transform.position.z);

        GameObject newCoin = Instantiate(coinPrefab, pos, Quaternion.identity);

        float randomStartingSpeedZ = Random.Range(maxStartingSpeed.z/2, maxStartingSpeed.z);
        Vector2 randomStartingSpeedXY = new Vector2(Random.Range(-maxStartingSpeed.x, maxStartingSpeed.x), Random.Range(-maxStartingSpeed.y, maxStartingSpeed.y));        

        CoinScript coinScript = newCoin.GetComponent<CoinScript>();

        coinScript.startingSpeed = randomStartingSpeedXY;
        coinScript.speedZ = randomStartingSpeedZ;
        coinScript.coinValue = value;
        
        newCoin.GetComponent<Rigidbody2D>().velocity = randomStartingSpeedXY;
    }
}
