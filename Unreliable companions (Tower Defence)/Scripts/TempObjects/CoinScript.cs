using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    [SerializeField]
    private float zMultiplyer;

    [SerializeField]
    private float gravityForce;

    [HideInInspector]
    public float speedZ;

    [HideInInspector]
    public Vector2 startingSpeed;

    private float posZ = 0;
    private float timeOnAir = 0;
    private int numOfBounces = 0;
    private bool still = false;

    public int coinValue;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        if(still) { return; }

        timeOnAir += Time.deltaTime;

        //vertical throw h = vt - 1/2gt2 
        posZ = speedZ * timeOnAir - (gravityForce / 2 * timeOnAir * timeOnAir);

        if (posZ < 0)
        {
            posZ = 0;
            speedZ /= 2;
            timeOnAir = 0;
            numOfBounces++;

            if (numOfBounces >= 5)
            {
                rigidBody.velocity = Vector2.zero;
                still = true;

                return;
            }
        }

        float yDistortion = (speedZ - gravityForce * timeOnAir) * zMultiplyer;

        rigidBody.velocity = startingSpeed + new Vector2(0, yDistortion);
    }
}
