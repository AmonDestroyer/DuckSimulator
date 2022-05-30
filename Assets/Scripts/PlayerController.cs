using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool debug = false;

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
    public float meleeDamage = 0.7f;
    public float meleeForce = 42.0f;
    public bool enableFire = true;
    public PlayerInput playerInput;
    public GameObject meleeScope;
    public Transform startSpawnPoint; // default spawn point
    public GameObject projectile;

    private static GameObject sampleInstance;
    private Rigidbody player;
    private Animator m_Animator;
    public Transform spawnPoint;
    private Transform m_ProjectileOrigin;
    private Transform m_ProjectileAnchor;
    // movement variables
    private Vector3 movement;
    private float movementX, movementY;
    private float lookX, lookY;
    // jump variables
    private bool isGlide;
    private int jumpCurrent;
    private float gravOpposite;
    private Vector3 gravStr;
    private InputAction jumpAction;
    private bool doJump;
    private bool isJumpPressed;
    private InputAction sprintAction;
    private bool isSprintPressed;
    private InputAction crouchAction;
    private bool isCrouchPressed;
    private float powerSlideTimerMax = 0.5f;
    private float powerSlideTimer = 0.0f;
    private InputAction fireAction;
    private bool doFire;
    private bool isFirePressed;
    private bool doMelee;
    //private PlayerShootObserver m_PlayerShootObserver;
    private MeleeObserver m_PlayerMeleeObserver;
    private MeleeScope m_PlayerMeleeScope;
    private Ejaculator m_Ejaculator;
    // for linear interpolation shots (i.e. charge shots)
    private float m_Firepower_lower = 500f;
    private float m_Firepower_upper = 2000f;
    private float m_LMBpress_max = 1.0f;
    private float m_LMBpress = 0.0f;
    private bool m_charge = false;
    private float m_chargeResetTime = 0.0f;
    private float m_chargeResetTimeMax = 1.0f;
    private bool resetSlider = false;
    // UI Update items
    private Slider m_slider;

    // Menu Items
    private Canvas m_ExitMenu;
    private Canvas m_HUD;

    // Start is called before the first frame update
    void Awake()
    {
        jumpAction = playerInput.currentActionMap["Jump"];
        fireAction = playerInput.currentActionMap["Fire"];
        sprintAction = playerInput.currentActionMap["Sprint"];
        crouchAction = playerInput.currentActionMap["Crouch"];
        startSpawnPoint = transform;
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
        //Locking cursor
        Cursor.lockState = CursorLockMode.Locked;
        //Setting Player basics
        gravStr = new Vector3(0, gravityStrength, 0);
        player = GetComponent<Rigidbody>();
        spawnPoint = startSpawnPoint;
        //Getting UI
        healthBar = GameObject.Find("HealthBarInner").GetComponent<Image>();
        m_slider = GameObject.Find("PowerMeter").GetComponent<Slider>();
        //Initial conditions set
        OnGroundTouch();
        SetDefaultMovement();

        //Getting Menu Canvas
        m_ExitMenu = GameObject.Find("ExitMenu").GetComponent<Canvas>();
        m_HUD = GameObject.Find("HUD").GetComponent<Canvas>();
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
        isSprintPressed = false;
        isCrouchPressed = false;
    }
    // JUMP functions

    private void OnGroundTouch()
    {
        // things to happen when player touches ground objects
        jumpCurrent = 0;
        isGlide = false;
    }

    void JumpAction()
    {
        //Make new velocity vector, note the players x and z velocity shouldn't
        //change
        Vector3 jump = new Vector3(player.velocity.x, jumpHeight, player.velocity.z);
        player.velocity = jump;
        jumpCurrent += 1; // used to limit number of jumps
    }

    void GlideAction()
    {
        // what players do when gliding
        isGlide = true; // for update usage
        // stops vertical acceleration
        Vector3 vert_cancel = new Vector3(movementX, 0.0f, movementY);
        player.velocity = vert_cancel;
        // used for making gravity calculation over time
        gravOpposite = 1.0f;
    }

    void MeleeAction() {
        m_PlayerMeleeObserver.sourceColliders = m_PlayerMeleeScope.getTriggerList();
        m_PlayerMeleeObserver.CollisionCheck();
    }

    void FireAction() {
        m_Ejaculator.SetVelocity(Mathf.Lerp(m_Firepower_lower, m_Firepower_upper, (m_LMBpress/m_LMBpress_max)));
        m_Ejaculator.Ejaculate();
    }

    void OnJump()
    {

        jumpAction.started += context => {
            if(jumpCurrent < jumpNum) {
                doJump = true; // do a jump!
            }
            isJumpPressed = true; // as long as jump is held
        };
        jumpAction.canceled += context => {
            // if player lets go of jump, glide stops
            isJumpPressed = false;

        };

    }
    // END JUMP functions
    void OnFire() {
        //filler function - currently attached to LMB
        //m_PlayerShootObserver.RayCheck();
        if (enableFire) {
            fireAction.started += context => {
                m_LMBpress = 0.0f;
                m_charge = true;
                resetSlider = false;
                m_chargeResetTime = 0.0f;
            };
            fireAction.canceled += context => {
                resetSlider = true;
                m_charge = false;
                doFire = true;
            };
        }
    }

    void OnMelee() {
        doMelee = true;
    }

    // SPRINT functions
    void OnSprint()
    {
        sprintAction.started += context => {
            if(isCrouchPressed == false) {
                isSprintPressed = true;
                m_Animator.SetBool("Walk", false);
                m_Animator.SetBool("Run", true);
            }
        };
        sprintAction.canceled += context => {
            isSprintPressed = false;
            m_Animator.SetBool("Walk", true);
            m_Animator.SetBool("Run", false);
        };
    }

    // CROUCH functions
    void OnCrouch()
    {
        crouchAction.started += context => {
            isCrouchPressed = true;
            powerSlideTimer = 0.0f;
        };

        crouchAction.canceled += context => {
            isCrouchPressed = false;
            isSprintPressed = false;
        };
    }

    public void OnMenu()
    {
      if (Time.timeScale == 1.0f){
        Time.timeScale = 0.0f;
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        m_HUD.enabled = false;
        m_ExitMenu.enabled = true;
      }
      else {
        stdTime();
      }

    }

    /*
    Function used to correct UI and time to standard time.
    */
    public void stdTime()
    {
      Time.timeScale = 1.0f;
      playerInput.SwitchCurrentActionMap("Player");
      Cursor.lockState = CursorLockMode.Locked;
      m_HUD.enabled = true;
      m_ExitMenu.enabled = false;
    }

    void Update() {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Physics.gravity = gravStr;
        movement = transform.forward * movementY + transform.right * movementX;
        if(doJump) {
            JumpAction();
            doJump = false;
        }

        if(isJumpPressed && isCrouchPressed) { // checks for gliding
            GlideAction();
        } else {
            isGlide = false;
        }

        if(isGlide == true) {
            // for glide; might put elsewhere later on
            gravOpposite =  (((player.velocity.y) * (gravityStrength * player.mass))  / ((gravityStrength) * glideMulti)) * -1.0f;
            Vector3 glide = new Vector3(0.0f, gravOpposite, 0.0f);
            player.AddForce(glide, ForceMode.Force);
        }

        if(isSprintPressed && isCrouchPressed && !isGlide) {
            if(powerSlideTimer <= powerSlideTimerMax) { // power slide!
                powerSlideTimer += Time.deltaTime;
                player.MovePosition(player.position + movement * sprintSpeed * 2);
            } else {
                isSprintPressed = false;
                isCrouchPressed = false;
            }
        } else if(isSprintPressed == true && isGlide == false) {
            player.MovePosition(player.position + movement * sprintSpeed);
        } else if(isCrouchPressed == true && isGlide == false) {
            player.MovePosition(player.position + movement * crouchSpeed);
        } else {
            player.MovePosition(player.position + movement * walkSpeed);
        }
        if(m_charge) {
            if(m_LMBpress_max > m_LMBpress) {
                    m_LMBpress += Time.deltaTime;
                } else {
                    m_charge = false;
                }
            //Update UI
            m_slider.value = Mathf.Lerp(0.0f,1.0f,(m_LMBpress/m_LMBpress_max));
        }
        if(doFire) {
            FireAction();
            doFire = false;
        }
        if(doMelee) {
            MeleeAction();
            doMelee = false;
        }
        if(resetSlider) {
            if(m_chargeResetTimeMax > m_chargeResetTime) {
                m_chargeResetTime += Time.deltaTime;
            } else {
                resetSlider = false;
                m_slider.value = 0;
            }
        }
        // animation components (messy rn and impromptu)
        bool moving = (movementY > 0) || (movementX > 0);
        if(moving)
        {
            if(isSprintPressed == true) {
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
            Debug.Log("Touched the ground!");
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

    public void Respawn() {
        player.position = spawnPoint.position;
    }

    void OnDisable() {
        Debug.Log("Script disabled");
    }

    void OnEnable() {
        Debug.Log("Script enabled");
    }

}
