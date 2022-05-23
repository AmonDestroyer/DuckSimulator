using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
Description: Adjuststs camera based on follow target part of the player
References:
- Creating a Third Person Camera using Cinemachine in Unity! (Tutorial)
  - Creator: Unity
  - Link: https://www.youtube.com/watch?v=537B1kJp9YQ
*/

public class PlayerCameraController : MonoBehaviour
{

    public PlayerInput playerInput;
    public GameObject followTransform;
    public GameObject projectileTransform;
    public Camera camera;
    public float pitchSpeed = 0.3f;
    public float yawSpeed = 0.3f;

    private Vector2 _look;

    void OnLook(InputValue lookValue)
    {
        _look = lookValue.Get<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
      //Rotate the Follow Target transform based on the input
      followTransform.transform.rotation *= Quaternion.AngleAxis(_look.x * yawSpeed, Vector3.up);
      //yaw
      followTransform.transform.rotation *= Quaternion.AngleAxis(-_look.y * pitchSpeed, Vector3.right);
      projectileTransform.transform.rotation *= Quaternion.AngleAxis(-_look.y * pitchSpeed, Vector3.right);
      //pitch
      //Set basic look angles
      var angles = followTransform.transform.localEulerAngles;
      angles.z = 0;

      var angle = followTransform.transform.localEulerAngles.x;
      //Clamp the Up/Down rotation
      if (angle > 180 && angle < 350) //Up (Camera below player)
      {
          angles.x = 350;
      }
      else if(angle < 180 && angle > 70) //Down (Camera above player)
      {
          angles.x = 70;
      }
      var angles_copy = angles;
      angles_copy.x -= 50;
      followTransform.transform.localEulerAngles = angles;
      projectileTransform.transform.localEulerAngles = angles_copy; // for the bullets
      
      //Set the player rotation based on the look transform
      transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
      //reset the y rotation of the look transform
      followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
    }
}
