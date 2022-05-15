using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private MainMapLoader MML;
    private GameObject[] enemiesToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        MML = FindObjectOfType<MainMapLoader>();

        SpawnMonster();
    }

    private void SpawnMonster()
    {
        enemiesToSpawn = MML.enemies;

        float random = Random.Range(0, 100);

        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            if (random < (i + 1) * 100 / enemiesToSpawn.Length)
            {
                Instantiate(enemiesToSpawn[i], transform.position, Quaternion.identity, transform.parent.transform);

                break;
            }
        }

        Destroy(gameObject);
    }       
}
