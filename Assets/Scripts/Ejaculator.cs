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
        /*Vector3 y_axis = new Vector3(0.0f, shaft.position.y, 0.0f);
        Vector3 rotation_axis = Vector3.Cross(shaft.forward, y_axis);
        Debug.Log($"{rotation_axis}");
        cock.RotateAround(shaft.position, rotation_axis, );*/
        Debug.Log($"{shaft.rotation.eulerAngles}, {shaft.forward}");
        Vector3 true_forward = Quaternion.AngleAxis(shaft.rotation.eulerAngles.y, shaft.forward) * shaft.forward;
        cock.position = shaft.position + true_forward * 1.0f;
        //cock.position = shaft.position + true_forward * 1.0f;
        
    }

    public void ejaculate() {
        Quaternion semen_rotation = sperm.transform.rotation;
        GameObject semen = Instantiate(sperm, cock.position, semen_rotation);
        Vector3 direction =  cock.position - shaft.position;
        direction.Normalize();
        semen.GetComponent<Rigidbody>().AddRelativeForce(ejaculateVelocity * direction);
    }

    public void set_velocity(float new_velocity) {
        ejaculateVelocity = new_velocity;
    }
};
