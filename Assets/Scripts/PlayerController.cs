using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerController : MonoBehaviour
{
    public float jumpHeight = 10.0f;
    public float walkSpeed = 0.2f;
    public float sprintSpeed = 0.4f;
    public float crouchSpeed = 0.1f;
    public float gravityStrength = -25.0f; // GRAVITY IS CURRENTLY UNIVERSAL; BE CAREFUL
    public int jumpNum = 2;
    public float glideMulti = 0.1f;
    public bool debug = false;
    public PlayerInput playerInput;
    public GameObject meleeScope;

    private Rigidbody player;
    private Animator m_Animator;
    // movement variables
    private Vector3 movement;
    private float movementX, movementY;
    private float lookX, lookY;
    private bool isSprint;
    private bool isCrouch;
    // jump variables
    private bool onGround;
    private bool isGlide;
    private int jumpCurrent;
    private float gravOpposite;
    private Vector3 gravStr;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private PlayerShootObserver m_PlayerShootObserver;
    private MeleeObserver m_PlayerMeleeObserver;
    private MeleeScope m_PlayerMeleeScope;

    // Start is called before the first frame update
    void Awake()
    {
        jumpAction = playerInput.currentActionMap["Jump"];
        sprintAction = playerInput.currentActionMap["Sprint"];
        crouchAction = playerInput.currentActionMap["Crouch"];
        player = GetComponent<Rigidbody>();
        m_Animator = GetComponentInChildren(typeof(Animator)) as Animator;

        m_PlayerShootObserver = new PlayerShootObserver();
        m_PlayerShootObserver.targetRange = 25.0f;
        m_PlayerShootObserver.targetTag = "Enemy";
        m_PlayerShootObserver.sourceTransform = GetComponent<Transform>();
        
        m_PlayerMeleeScope = meleeScope.GetComponentInChildren<MeleeScope>(); 
        m_PlayerMeleeObserver = new MeleeObserver();
        m_PlayerMeleeObserver.targetRange = 5.0f;
        m_PlayerMeleeObserver.targetTag = "Enemy";
        m_PlayerMeleeObserver.sourceTransform = GetComponent<Transform>();

        Debug.Log($"{m_PlayerShootObserver.targetTag}, {m_PlayerShootObserver.targetRange}");
    }

    void Start()
    {
        gravStr = new Vector3(0, gravityStrength, 0);
        player = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        OnGroundTouch();
        SetDefaultMovement();
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        //if (debug)
            //Debug.Log($"Input: ({movementX},{movementY})");
    }

    private void SetDefaultMovement()
    {
        // Resets all movement factors on player
        player.velocity = Vector3.zero;
        isSprint = false;
        isCrouch = false;
    }
    // JUMP functions

    private void OnGroundTouch()
    {
        // things to happen when player touches ground objects
        onGround = true;
        jumpCurrent = 0;
        isGlide = false;
    }

    void JumpAction()
    {
        Vector3 jump = new Vector3(0.0f, jumpHeight, 0.0f);
        player.AddForce(jump, ForceMode.Impulse);
        onGround = false; // used to reset jumps AND for glide
        jumpCurrent += 1; // used to limit number of jumps
        //if(debug)
            //Debug.Log($"(Jump #, isGround): ({jumpCurrent}, {onGround})");
    }

    void GlideAction()
    {
        // what players do when gliding
        isGlide = true; // for update usage
        jumpCurrent = jumpNum; // no more jumping while gliding
        // stops vertical acceleration
        Vector3 vert_cancel = new Vector3(movementX, 0.0f, movementY);
        player.velocity = vert_cancel;
        // used for making gravity calculation over time
        gravOpposite = 0.0f;
    }

    void OnJump()
    {
        // if already in the air and out of jumps, glide instantly
        jumpAction.started += context => {
            if(onGround == false && jumpCurrent == jumpNum) {
                GlideAction();
            }
        };
        jumpAction.performed += context => {
            if (context.interaction is HoldInteraction) {
                // if player holds jump, glide starts; no more jumps allowed!
                if(onGround == false && isGlide == false) {
                    GlideAction();
                    if(debug)
                        Debug.Log($"Is gliding!");
                }
            }
            else if (context.interaction is PressInteraction) {
                if(onGround == true || jumpCurrent < jumpNum) {
                    // if player taps jump, and has jumps left, jump!
                    JumpAction();
                }
            }
        };
        jumpAction.canceled += context => {
            // if player lets go of jump, glide stops
            isGlide = false;
        };

    }
    // END JUMP functions
    void OnFire() {
        //filler function - currently attached to LMB
        m_PlayerShootObserver.RayCheck();
    }

    void OnMelee() {
        //filler function - currently attached to 'v'
        m_PlayerMeleeObserver.sourceColliders = m_PlayerMeleeScope.TriggerList;
        m_PlayerMeleeObserver.CollisionCheck();
    }

    // SPRINT functions
    void OnSprint()
    {
        sprintAction.started += context => {
            isSprint = true;
            m_Animator.SetBool("Walk", false);
            m_Animator.SetBool("Run", true);
        };

        sprintAction.canceled += context => {
            isSprint = false;
            m_Animator.SetBool("Walk", true);
            m_Animator.SetBool("Run", false);
        };
    }

    // CROUCH functions
    void OnCrouch()
    {
        crouchAction.started += context => {
            isCrouch = true;
        };

        crouchAction.canceled += context => {
            isCrouch = false;
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Physics.gravity = gravStr;
        movement = transform.forward * movementY + transform.right * movementX;
        if(isGlide == true) {
            // for glide; might put elsewhere later on
            gravOpposite =  (((player.velocity.y) * (gravityStrength * player.mass))  / ((gravityStrength) * glideMulti)) * -1.0f;
            Vector3 glide = new Vector3(0.0f, gravOpposite, 0.0f);
            player.AddForce(glide, ForceMode.Force);
        }

        if(isSprint == true)
        {
            player.MovePosition(player.position + movement * sprintSpeed);
        }
        else if(isCrouch == true)
        {
            player.MovePosition(player.position + movement * crouchSpeed);
        }
        else
        {
            player.MovePosition(player.position + movement * walkSpeed);
        }
        // animation components (messy rn and impromptu)
        bool moving = (movementY > 0) || (movementX > 0);
        if(moving) 
        {
            if(isSprint == true) {
                m_Animator.SetBool("Walk", !moving);
                m_Animator.SetBool("Run", moving);
            } 
            else 
            {
                m_Animator.SetBool("Walk", moving);
                m_Animator.SetBool("Run", !moving);
            }
        }
        else
        {
            m_Animator.SetBool("Walk", moving);
            m_Animator.SetBool("Run", moving);
        }
    }
    
    void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Ground")) {
            OnGroundTouch();
        }
    }
}
