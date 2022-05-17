using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeObserver : Observer
{   //making the below 'meleeForce' public does nothing since this is not a 'MonoBehaviour' class...
    float meleeForce;
    float damage;
    GameObject player;


    public MeleeObserver(GameObject thisGameObject, float meleeForce, float damage){
        this.meleeForce = meleeForce;
        this.player = thisGameObject;
        this.damage = damage;
    }

    public override Ray CreateRay() {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0));
        return ray;
    }

    public override void TargetSpotted() {
        Debug.Log("Player Melee Hit!");
    }

    //below method added by Alder 5/10/22 to implement melee force
    public override void ApplyAttackForce(Collider target){
        target.attachedRigidbody.AddForce(player.transform.forward * this.meleeForce, ForceMode.Impulse);
        EnemyBase scriptInstance = target.GetComponent<EnemyBase>();
        scriptInstance.ApplyDamage(this.damage);
    }
}
