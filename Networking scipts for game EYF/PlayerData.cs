using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public string name;
    public GameObject gameObject;
    public float chaseRadius = 2;

    public PlayerData(string inputName = "unknown", GameObject inputGameObject = null)
    {
        name = inputName;
        gameObject = inputGameObject;
    }
}
