using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeObserver : Observer
{   //making the below 'meleeForce' public does nothing since this is not a 'MonoBehaviour' class...
    float meleeForce = 42.0f;
    GameObject player;

    public override Ray CreateRay() {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0));
        return ray;
    }

    public override void TargetSpotted() {
        Debug.Log("Melee Hit!");
    }

    //below method added by Alder 5/10/22 to implement melee force
    public override void ApplyAttackForce(Collider target){
        player = GameObject.Find("Player");
        target.attachedRigidbody.AddForce(player.transform.forward * meleeForce, ForceMode.Impulse);
    }
}
