using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPlayerScript : MonoBehaviour
{
    private float moveInput;
    private Joystick movementJoystick;
    public RectTransform fireButton;
    public RectTransform jumpButton;
    public RectTransform abitlityButton;

    public Rigidbody2D pRigidBody;
    public Animator animator;

    [HideInInspector]
    public PlayerMovement PMS;
    public PlayerAttack PAS;
    public PlayerAbility PABS;
    public PlayerInventory PIS;
    public PlayerHpScript PHPS;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();

        PMS = GetComponent<PlayerMovement>();
        PAS = GetComponent<PlayerAttack>();
        PABS = GetComponent<PlayerAbility>();
        PIS = GetComponent<PlayerInventory>();
        PHPS = GetComponent<PlayerHpScript>();

        if (Settings.mobileMode)
        {
            movementJoystick = GameObject.Find("MovemantJoystick").GetComponent<Joystick>();
            fireButton = movementJoystick.transform.parent.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
            jumpButton = movementJoystick.transform.parent.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
            abitlityButton = movementJoystick.transform.parent.transform.GetChild(2).gameObject.GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        InputFunctions();
    }
    private void FixedUpdate()
    {
        PMS.MoveWithPhysics(moveInput);
    }

    public void TakeDamage(float damageAmout)
    {
        Debug.Log("hurt");
        PHPS.TakeDamageEffect(damageAmout);       
    }

    private void InputFunctions()
    {
        if (Settings.mobileMode)
        {
            InputMobile();
        } else
        {
            InputPC();
        }
    }

    private void InputMobile()
    {

        //movement
        if (movementJoystick.Horizontal >= 0.2f)
        {
            moveInput = 1;
        }
        else if (movementJoystick.Horizontal <= -0.2f)
        {
            moveInput = -1;
        }

        if (movementJoystick.Vertical <= -0.5f)
        {
            PMS.InteractWithFloor();
        }

        if (Input.touchCount > 0)
        {
            Touch currentTouch = Input.GetTouch(0);
            Debug.Log(Vector2.Distance(currentTouch.position, fireButton.position));

            if (Vector2.Distance(currentTouch.position, fireButton.position) <= 100)
            {
                if (PIS.curretObject != null) 
                {
                    PIS.Intertact(); 
                } else
                {
                    PAS.Attack();
                }

            } 
            else if (Vector2.Distance(currentTouch.position, jumpButton.position) <= 100)
            {
                PMS.Jump();
            }
            else if (Vector2.Distance(currentTouch.position, abitlityButton.position) <= 100)
            {
                PABS.ActivateAbility();
            }
        }     



    }

     private void InputPC()
    {
        if (Input.GetMouseButton(0))
        {
            PAS.Attack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            PABS.ActivateAbility();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PIS.Intertact();
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            PMS.InteractWithFloor();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PMS.Jump();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            PMS.ShorterJump();
        }
    }
}

