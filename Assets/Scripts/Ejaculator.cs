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
        //FOLLOW METHOD
        Vector3 forward_shaft = new Vector3(shaft.forward.x, 0.0f, shaft.forward.z); // calculates where to place cock
        //cock.position = shaft.position + shaft.forward * 1.0f; // places cock in front of object
        
        cock.position = shaft.position + Vector3.up + forward_shaft;
    }

    public void Ejaculate(float damage, Vector3 target) {  // fires objects
        Quaternion semen_rotation = sperm.transform.rotation; // takes rotation of object (already adjusted manually)
        //Vector3 direction =  cock.position - shaft.position; // calculates direction of launch and...
        //direction.Normalize();                               // turns into a direction vector
        Vector3 direction = Vector3.up;
        GameObject semen = Instantiate(sperm, cock.position, semen_rotation); // makes a projectile object
        semen.GetComponent<Rigidbody>().AddForce(500f * direction); // launches projectile along trajectory
        Projectile semenControl = semen.GetComponent<Projectile>();
        semenControl.speed = ejaculateVelocity;
        semenControl.targetPosition = target;
    }

    public void SetVelocity(float new_velocity) { // new velocity for firing things
        ejaculateVelocity = new_velocity;
        Debug.Log($"{ejaculateVelocity}");
    }
};
