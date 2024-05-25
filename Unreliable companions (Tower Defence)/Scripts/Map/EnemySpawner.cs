using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Transform[] path;

    private Vector2[] waypoints;

    [SerializeField]
    private Wave[] waves;

    private int waveIndex = 0;
    private bool spawning = false;

    private int enemyTypeIndex = 0;
    private int enemyTypeSpawned = 0;

    private float timeToSpawnNewEnemy = 0;

    [Header("GameOverscreen")]
    [SerializeField]
    private GameObject endScreen;

    [SerializeField]
    private Text WinText;

    [SerializeField]
    private Animator endScreenAnimator;

    [SerializeField]
    private Image countdownImage;

    [SerializeField]
    private float startingTime;

    private float timeToNextWave = 0;
    private float timeToNextWaveMax = 0;

    public bool gameOver = false;

    private void Awake()
    {
        waypoints = new Vector2[path.Length];

        for (int i = 0; i < path.Length; i++)
        {
            waypoints[i] = path[i].position;
        }

        path = null;
    }

    private void Update()
    {
        TimerTick();

        SpawnWave();

        CountDownTimeTonextWave();
    }

    private void CountDownTimeTonextWave()
    {
        if (timeToNextWave > 0)
        {
            timeToNextWave -= Time.deltaTime;

            if (timeToNextWave <= 0)
            {
                timeToNextWave = 0;
                StartNewWave();
                return;
            }

            countdownImage.fillAmount = timeToNextWave / timeToNextWaveMax;
        }
    }

    public void StartSpawning()
    {
        timeToNextWave = startingTime;
        timeToNextWaveMax = startingTime;
    }

    public void StartNewWave()
    {
        if (spawning) { return; };
        if (waveIndex >= waves.Length)
        {
            Win();
            return;
        }        

        spawning = true;

        timeToNextWave = 0;
        timeToNextWaveMax = 0;

        countdownImage.fillAmount = 1;

        SoundManager.instance.PlaySound("NewWave");
    }

    private void TimerTick()
    {
        if (timeToSpawnNewEnemy <= 0) { return; }

        timeToSpawnNewEnemy -= Time.deltaTime;

        if(timeToSpawnNewEnemy > 0) { return; }

        timeToSpawnNewEnemy = 0;
    }

    private void SpawnWave()
    {
        if (!spawning) { return; }
        if(timeToSpawnNewEnemy > 0) { return; }
        if(gameOver) { return; }

        GameObject newEnemyPrefab = waves[waveIndex].enemies[enemyTypeIndex].enemyObject;

        newEnemyPrefab.GetComponent<EnemyMovement>().waypoints = waypoints;

        Vector3 position = new Vector3(transform.position.x, transform.position.y, newEnemyPrefab.transform.position.z);

        Instantiate(newEnemyPrefab, position, Quaternion.identity);

        

        //array mindfuck!!!!

        //check if anythying is on end of array

        enemyTypeSpawned++;

        timeToSpawnNewEnemy += waves[waveIndex].enemies[enemyTypeIndex].spawnRate;

        if (enemyTypeSpawned >= waves[waveIndex].enemies[enemyTypeIndex].numberEnemies)
        {
            //time to spawn new Group - time to spawn new enemy from group, added befoure;
            timeToSpawnNewEnemy += waves[waveIndex].enemies[enemyTypeIndex].spawnRate - waves[waveIndex].enemies[enemyTypeIndex].spawnRate;

            enemyTypeIndex++;
            enemyTypeSpawned = 0;

            if (enemyTypeIndex >= waves[waveIndex].enemiesLength())
            {
                spawning = false;

                enemyTypeIndex = 0;
                enemyTypeSpawned = 0;
                timeToSpawnNewEnemy = 0;

                waveIndex++;
                if (waveIndex < waves.Length)
                {
                    timeToNextWave = 20;
                    timeToNextWaveMax = 20;
                }
            }
        }
    }

    private void Win()
    {
        WinText.text = "Victory";

        endScreen.SetActive(true);

        endScreenAnimator.SetTrigger("FadeIn");
    }


    [System.Serializable]
    public struct Wave 
    {
        public EnemyType[] enemies;

        public int enemiesLength()
        {
            int x = 0;

            foreach(EnemyType enemyType in enemies)
            {
                x++;
            }

            return x;
        }
    }

    [System.Serializable]
    public struct EnemyType 
    {
        public GameObject enemyObject;
        public int numberEnemies;
        public float spawnRate;
        public float timeAfterSpawnNewEnemyType;
    }
}
