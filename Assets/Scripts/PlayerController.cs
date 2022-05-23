using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // health bar control implemented with help of below tutorial
    // https://medium.com/swlh/game-dev-how-to-make-health-bars-in-unity-from-beginner-to-advanced-9a1d728d0cbf
    public float health = 1.0f; // 1 = 100 % full, .5 = 50%, etc...
    public Image healthBar;

    public float jumpHeight = 10.0f;
    public float walkSpeed = 0.2f;
    public float sprintSpeed = 0.4f;
    public float crouchSpeed = 0.1f;
    public float gravityStrength = -25.0f; // GRAVITY IS CURRENTLY UNIVERSAL; BE CAREFUL
    public int jumpNum = 2;
    public float glideMulti = 0.1f;
    public float terminalVelocity = -75.0f;
    public bool debug = false;
    public float meleeDamage = 0.7f;
    public float meleeForce = 42.0f;
    public bool enableFire = true;
    public PlayerInput playerInput;
    public GameObject meleeScope;
    public Transform startSpawnPoint;
    public GameObject projectile;

    private Rigidbody player;
    private Animator m_Animator;
    private Transform spawnPoint;
    private Transform m_ProjectileOrigin;
    private Transform m_ProjectileAnchor;
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
    private InputAction fireAction;
    //private PlayerShootObserver m_PlayerShootObserver;
    private MeleeObserver m_PlayerMeleeObserver;
    private MeleeScope m_PlayerMeleeScope;
    private Ejaculator m_Ejaculator;
    // for linear interpolation shots (i.e. charge shots)
    private float m_Firepower_lower = 100f;
    private float m_Firepower_upper = 2000f;
    private float m_LMBpress_max = 2.0f;
    private float m_LMBpress = 0.0f;
    private bool m_charge = false;

    // Start is called before the first frame update
    void Awake()
    {
        jumpAction = playerInput.currentActionMap["Jump"];
        fireAction = playerInput.currentActionMap["Fire"];
        sprintAction = playerInput.currentActionMap["Sprint"];
        crouchAction = playerInput.currentActionMap["Crouch"];
        player = GetComponent<Rigidbody>();
        m_Animator = GetComponentInChildren(typeof(Animator)) as Animator;
        /*
        m_PlayerShootObserver = new PlayerShootObserver();
        m_PlayerShootObserver.targetRange = 25.0f;
        m_PlayerShootObserver.targetTag = "Enemy";
        m_PlayerShootObserver.sourceTransform = GetComponent<Transform>();
        */
        m_PlayerMeleeScope = meleeScope.GetComponentInChildren<MeleeScope>();
        m_PlayerMeleeObserver = new MeleeObserver(gameObject, meleeForce, meleeDamage);
        m_ProjectileOrigin = transform.Find("ProjectileOrigin");
        m_ProjectileAnchor = transform.Find("ProjectileAnchor");
        m_Ejaculator = gameObject.AddComponent<Ejaculator>() as Ejaculator;
        m_Ejaculator.cock = m_ProjectileOrigin;
        m_Ejaculator.sperm = projectile;
        m_Ejaculator.shaft = m_ProjectileAnchor;
        m_PlayerMeleeObserver.targetRange = 5.0f;
        m_PlayerMeleeObserver.targetTag = "Enemy";
        m_PlayerMeleeObserver.sourceTransform = GetComponent<Transform>();

        //Debug.Log($"{m_PlayerShootObserver.targetTag}, {m_PlayerShootObserver.targetRange}");
    }

    void Start()
    {
        gravStr = new Vector3(0, gravityStrength, 0);
        player = GetComponent<Rigidbody>();
        healthBar = GameObject.Find("HealthBarInner").GetComponent<Image>();
        Cursor.lockState = CursorLockMode.Locked;
        spawnPoint = startSpawnPoint;
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
        //Make new velocity vector, note the players x and z velocity shouldn't
        //change
        Vector3 jump = new Vector3(player.velocity.x, jumpHeight, player.velocity.z);
        player.velocity = jump;
        onGround = false; // used to reset jumps AND for glide
        jumpCurrent += 1; // used to limit number of jumps
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
                if(jumpCurrent < jumpNum) {
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
        //m_PlayerShootObserver.RayCheck();
        if (enableFire) {
            fireAction.started += context => {
                m_LMBpress = 0.0f;
            };
            fireAction.performed += context => {
                m_charge = true;
            };
            fireAction.canceled += context => {
                m_charge = false;
                m_Ejaculator.SetVelocity(Mathf.Lerp(m_Firepower_lower, m_Firepower_upper, (m_LMBpress/m_LMBpress_max)));
                m_Ejaculator.Ejaculate();
            };
        }
    }

    void OnMelee() {
        //filler function - currently attached to 'v'
        m_PlayerMeleeObserver.sourceColliders = m_PlayerMeleeScope.getTriggerList();
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

    void OnMenu()
    {
      if (Time.timeScale == 1.0f){
        Time.timeScale = 0.0f;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
      }
      else {
        Time.timeScale = 1.0f;
        playerInput.SwitchCurrentActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;
      }

    }

    void Update() {

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
        if(m_charge) {
            if(m_LMBpress_max > m_LMBpress) {
                    m_LMBpress += Time.deltaTime;
                } else {
                    m_charge = false;
                }
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

        //Set terminalVelocity
        if (player.velocity.y < terminalVelocity){
          Vector3 vel = player.velocity;
          vel.y = terminalVelocity;
          player.velocity = vel;
        }

        //EXAMPLE OF HOW YOU UPDATE HEALTH
        UpdateHealth();
        // in practice you wouldn't need to call UpdateHealth(); every frame,
        // you only need to call it after you modify the 'health' variable
        // e.g. when your player takes damage.
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Ground")) {
            OnGroundTouch();
        }
        if(other.gameObject.CompareTag("Death")){
            player.position = spawnPoint.position;
        }
        if(other.gameObject.CompareTag("Respawn")){
          spawnPoint = other.gameObject.transform;
          other.gameObject.SetActive(false);
        }
    }

    void UpdateHealth(){ //call this from update or fixedupdate
    //anytime after you change the 'health' variable
        healthBar.fillAmount = health;
    }

    public void ApplyDamage(float damage){
        health -= damage;
        UpdateHealth();
    }
}
