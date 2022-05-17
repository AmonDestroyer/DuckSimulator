using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeObserver : Observer
{   //making the below 'meleeForce' public does nothing since this is not a 'MonoBehaviour' class...
    float meleeForce;
    float damage;
    GameObject enemy;

    //using a contstructor allows us to pass public variables from EnemyBase to this class
    public EnemyMeleeObserver(GameObject thisGameObject, float meleeForce, float damage){
        this.meleeForce = meleeForce;
        this.enemy = thisGameObject;
        this.damage = damage;
    }


    public override Ray CreateRay() {
        //Ray ray = Camera.main.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0));
        return new Ray(this.enemy.transform.position, this.enemy.transform.forward);
    }

    public override void TargetSpotted() {
        Debug.Log("Enemy Melee Hit!");
    }

    //below method added by Alder 5/10/22 to implement melee force
    public override void ApplyAttackForce(Collider target){
        //player = GameObject.Find("Player");
        target.attachedRigidbody.AddForce(this.enemy.transform.forward * this.meleeForce, ForceMode.Impulse);
        PlayerController scriptInstance = target.GetComponent<PlayerController>();
        scriptInstance.ApplyDamage(this.damage);
    }
}