using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class IfNotSeverDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (SettingsScript.singlePlayerMode) { return; }

        Destroy(gameObject);
    }
}
