using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 0.2f;
    public float height = 10.0f;
    public int jumpNum = 2;
    public bool debug = false;
    public PlayerInput playerInput;

    private Rigidbody player;
    private Vector3 movement;
    private float movementX;
    private float movementY;
    private bool onGround;
    private int jumpCurrent;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody>();
        onGround = true;
        jumpCurrent = 0;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        if (debug)
            Debug.Log($"Input: ({movementX},{movementY})");
        movement = new Vector3(movementX, 0.0f, movementY);

    }

    void OnJump()
    {
        if(onGround == true || jumpCurrent < jumpNum) {
            Vector3 jump = new Vector3(0.0f, height, 0.0f);
            player.AddForce(jump, ForceMode.Impulse);
            onGround = false;
            jumpCurrent += 1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        player.MovePosition(player.position + movement * speed);
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Ground")) {
            onGround = true;
            jumpCurrent = 0;
        }
    }
}
