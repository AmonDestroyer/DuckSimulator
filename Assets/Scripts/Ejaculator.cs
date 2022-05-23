using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ejaculator: MonoBehaviour
{
    public Transform cock; // object giving position of launching
    public Transform shaft; // actual body of thing launching object
    public GameObject sperm; // object being launched
    public float ejaculateVelocity = 1000f;

    void Update() {
        Vector3 forward_shaft = new Vector3(shaft.forward.x, 0.0f, shaft.forward.z);
        cock.position = shaft.position + shaft.forward * 1.0f;
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

    public void ejaculate() {
        Quaternion semen_rotation = sperm.transform.rotation;
        GameObject semen = Instantiate(sperm, cock.position, semen_rotation);
        Vector3 direction =  cock.position - shaft.position;
        direction.Normalize();
        semen.GetComponent<Rigidbody>().AddForce(ejaculateVelocity * direction);
    }

    public void set_velocity(float new_velocity) {
        ejaculateVelocity = new_velocity;
    }
};
