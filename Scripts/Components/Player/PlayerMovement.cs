using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public ParticleSystem DustWhenFall;
    private MainPlayerScript MPS;
    private Rigidbody2D rigidBody;

    private bool onGround = false;

    private float movementInput;

    //movement 
    public float baseSpeed = 20;
    public float jumpForce = 75;

    public float acceleration = 7;
    public float decceleration = 7;
    public float velPower = 0.9f;
    public float frictionAmount = 0.2f;
    public float fallMultiplier = 1.5f;

    private float gravityDefaultScale;
    private float timeInAir = 0;
    private float tolleranceTime = 0.15f;

    private float activeJumpCoolDown = 0;
    private float jumpCoolDown = 0.2f;


    private void Start()
    {
        MPS = GetComponent<MainPlayerScript>();
        rigidBody = MPS.pRigidBody;
        gravityDefaultScale = rigidBody.gravityScale;
    }

    private void Update()
    {
        IsOnGround();
    }

    public void MoveWithPhysics(float moveInput)
    {
        movementInput = moveInput;
        Move();
        Friction();

        if(!onGround)
        {
            timeInAir += Time.deltaTime;
        } else
        {
            timeInAir = 0;
        }

        ChangeMovemantAnim();
    }

    public void Jump()
    {
        if ((onGround || timeInAir < tolleranceTime) && activeJumpCoolDown <= 0)
        {
            rigidBody.AddForce(Vector2.up * (jumpForce + rigidBody.velocity.y * -1), ForceMode2D.Impulse);
            Debug.Log("jump");
            activeJumpCoolDown += jumpCoolDown;
        }
    }

    public void ShorterJump()
    {
        if (rigidBody.velocity.y > 0)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y * 0.5f);
        }
    }
    private void Move()
    {
        float targetSpeed = movementInput * baseSpeed;

        float speedDif = targetSpeed - rigidBody.velocity.x;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : decceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        rigidBody.AddForce(Vector2.right * movement);

        SwapCharacterDirection();
    }

    private void SwapCharacterDirection()
    {
        if (movementInput > 0.1)
        {
            MPS.PAS.FlipWeapon(true);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (movementInput < -0.1)
        {
            MPS.PAS.FlipWeapon(false);
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Friction()
    {
        if (onGround && Mathf.Abs(movementInput) < 0.01f)
        {
            float frictionForce = Mathf.Min(Mathf.Abs(rigidBody.velocity.x), frictionAmount) * Mathf.Sign(rigidBody.velocity.x);

            rigidBody.AddForce(Vector2.right * -frictionForce, ForceMode2D.Impulse);
        }
    }

    public void ChangeMovemantAnim()
    {
        MPS.animator.SetFloat("VelocityX", Mathf.Abs(rigidBody.velocity.x));
        MPS.animator.SetFloat("VelocityY", rigidBody.velocity.y);
    } 

    public void InteractWithFloor()
    {
        Collider2D[] currentColider = Physics2D.OverlapBoxAll((Vector2)transform.position + new Vector2(0, -2), new Vector2(1, 0.25f), 0, ~(1 << 6));

        foreach(Collider2D CC in currentColider)
        {
            HlafFloorScript HFS;

            CC.gameObject.TryGetComponent<HlafFloorScript>(out HFS);

            if (HFS != null)
            {
                HFS.Disable();
            }
        }        
    }

    private void IsOnGround()
    {
        bool wasOnFGround = onGround;

        onGround = Physics2D.OverlapBox((Vector2)transform.position + new Vector2(0, -2), new Vector2(1, 0.25f), 0, ~(1 << 6)) != null ? true : false;

        //trigger enter
        if (!wasOnFGround && onGround)
        {
            DustWhenFall.Play();
        }

        if (activeJumpCoolDown > 0)
        {
            activeJumpCoolDown -= Time.deltaTime;
        }
    }
}
