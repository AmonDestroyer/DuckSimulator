using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 0.2f;
    public float lookSpeed = 100.0f;
    public float jumpHeight = 10.0f;
    public float gravityStrength = -1.0f;
    public int jumpNum = 2;
    public bool debug = false;
    public PlayerInput playerInput;
    

    private Rigidbody player;
    private Vector3 movement;
    private Quaternion look = Quaternion.identity;
    private float movementX, movementY;
    private float lookX, lookY;
    private bool onGround;
    private int jumpCurrent;
    private Vector3 gravStr;

    // Start is called before the first frame update
    void Start()
    {
        gravStr = new Vector3(0, gravityStrength, 0);
        player = GetComponent<Rigidbody>();
        onGround = true;
        jumpCurrent = 0;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        if (debug)
            Debug.Log($"Input: ({movementX},{movementY})");
    }

    void OnLook(InputValue lookValue)
    {
        Vector2 lookVector = lookValue.Get<Vector2>();
        lookX = lookVector.x;
        lookY = lookVector.y; //Right now up and down look is not presently working
        if (debug)
            Debug.Log($"Look: ({lookX},{lookY})");
        Vector3 rotation = Quaternion.AngleAxis(lookX, transform.up) * transform.forward;
        Vector3 lookDirection = Vector3.RotateTowards(transform.forward, rotation, lookSpeed * Time.deltaTime, 0f);
        lookDirection.Normalize();
        look = Quaternion.LookRotation(lookDirection);
    }

    void OnJump()
    {
        if(onGround == true || jumpCurrent < jumpNum) {
            Vector3 jump = new Vector3(0.0f, jumpHeight, 0.0f);
            player.AddForce(jump, ForceMode.Impulse);
            onGround = false;
            jumpCurrent += 1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Physics.gravity = gravStr;
        movement = transform.forward * movementY + transform.right * movementX;
        player.MovePosition(player.position + movement * walkSpeed);
        player.MoveRotation(look);
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Ground")) {
            onGround = true;
            jumpCurrent = 0;
        }
    }
}
