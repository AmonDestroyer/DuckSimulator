using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ejaculator: MonoBehaviour
{
    public Transform cock; // object giving position of launching
    public Transform shaft; // actual body of thing launching object
    public GameObject sperm; // object being launched
    public float ejaculateVelocity = 1000f; // velocity

    void Update() { // constantly adjusts position of aiming anchors
        Vector3 forward_shaft = new Vector3(shaft.forward.x, 0.0f, shaft.forward.z); // calculates where to place cock
        cock.position = shaft.position + shaft.forward * 1.0f; // places cock in front of object
        // VESTIGIAL CODE
        //Vector3 rotation_axis = Vector3.Cross(shaft.forward, Vector3.up);
        //Debug.Log($"{shaft.forward}; {rotation_axis}");
        //float shaft_angle = shaft.localEulerAngles.x;
        
        //Debug.Log($"{rotation_axis}");
        //cock.RotateAround(shaft.position, rotation_axis, 100f * Time.deltaTime);
        //Debug.Log($"Position Changes: shaft: {shaft.position}; cock: {cock.position}. Rotation Changes: shaft: {shaft.localEulerAngles}; cock: {cock.localEulerAngles}");
        //Debug.Log($"Rotation Changes: shaft: {shaft.localEulerAngles}; cock: {cock.localEulerAngles}");
        //Debug.Log($"{shaft_angle}, {rotation_axis}, {forward_shaft}");
        //Debug.Log($"{pump}");
        //Vector3 true_forward = Quaternion.AngleAxis(0.0f, rotation_axis) * shaft.forward;
        
    }

    public void ejaculate() {  // fires objects
        Quaternion semen_rotation = sperm.transform.rotation; // takes rotation of object (already adjusted manually)
        Vector3 direction =  cock.position - shaft.position; // calculates direction of launch and...
        direction.Normalize();                               // turns into a direction vector
        GameObject semen = Instantiate(sperm, cock.position, semen_rotation); // makes a projectile object
        semen.GetComponent<Rigidbody>().AddForce(ejaculateVelocity * direction); // launches projectile along trajectory
    }

    public void set_velocity(float new_velocity) { // new velocity for firing projectiles
        ejaculateVelocity = new_velocity;
    }
};
