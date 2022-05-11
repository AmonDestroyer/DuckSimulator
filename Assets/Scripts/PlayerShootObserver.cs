using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootObserver : Observer
{
    public override Ray CreateRay() {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0));
        return ray;
    }

    public override void TargetSpotted() {
        Debug.Log("Hit!");
    }

    public override void ApplyAttackForce(Collider target){
        // implement force you want to apply to enemy when they get shot here
    }
    
}
