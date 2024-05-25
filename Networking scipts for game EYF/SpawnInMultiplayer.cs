using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class SpawnInMultiplayer : MonoBehaviour
{
    public GameObject[] GameObjectsToSpawn;

    void Start()
    {
        if (!SettingsScript.singlePlayerMode)
        {
            foreach (GameObject GO in GameObjectsToSpawn)
            {
                GO.GetComponent<NetworkObject>().Spawn();
                Debug.Log("Spawnedder");
            }
        }
    }
}
