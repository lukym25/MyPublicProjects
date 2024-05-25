using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BuildingInfo : MonoBehaviour 
{
    public string buildingName, description;
    public Sprite sprite;

    public float damage, attackSpeed, range, projectileSpeed;

    public int level;

    [HideInInspector]
    public Vector2 position;    
}
