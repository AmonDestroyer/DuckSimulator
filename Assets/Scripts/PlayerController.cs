using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 0.2f;
    public float height = 10.0f;
    public bool debug = false;
    public PlayerInput palyerInput;

    private Rigidbody player;
    private Vector3 movement;
    private float movementX;
    private float movementY;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody>();
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
        Vector3 jump = new Vector3(0.0f, height, 0.0f);
        player.AddForce(jump, ForceMode.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        player.MovePosition(player.position + movement * speed);
    }
}
