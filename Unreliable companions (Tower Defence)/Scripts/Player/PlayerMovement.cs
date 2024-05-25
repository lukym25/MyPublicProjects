using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidBody;
    [SerializeField]
    private float momenentSpeed, acceleration, decceleration, velPower;

    // Update is called once per frame
    void Update()
    {
        PlayerMove(GetInput());
    }

    private Vector2 GetInput()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        return new Vector2(inputX, inputY);
    }

    private void PlayerMove(Vector2 dir)
    {
        rigidBody.velocity = dir * momenentSpeed;





        /*Vector2 targetSpeed = dir * momenentSpeed;

        Vector2 speedDif = targetSpeed - (Vector2)rigidBody.velocity;

        float accelRateX = Mathf.Abs(targetSpeed.x) > 0.01f ? acceleration : decceleration;

        float accelRateY = Mathf.Abs(targetSpeed.y) > 0.01f ? acceleration : decceleration;

        float movementX = speedDif.x * accelRateX;

        float movementY = speedDif.y * accelRateY;
        

        float movementX = Mathf.Pow(Mathf.Abs(speedDif.x) * accelRateX, velPower) * Mathf.Sign(speedDif.x);

        float movementY = Mathf.Pow(Mathf.Abs(speedDif.y) * accelRateY, velPower) * Mathf.Sign(speedDif.y);
        

        Vector2 movement = new Vector2(movementX, movementY);

        rigidBody.AddForce(movement);*/

        //swap sprite dir
    }
}
