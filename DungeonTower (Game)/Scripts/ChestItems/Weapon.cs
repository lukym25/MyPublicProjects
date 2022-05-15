using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    public bool melee;

    public float damage;
    public float attackRange;
    public float attackSpeed;

    public Sprite sprite;

    public GameObject Projectile;
    public float projectileSpeed;
}
