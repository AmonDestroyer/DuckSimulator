using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeObserver : Observer
{   //making the below 'meleeForce' public does nothing since this is not a 'MonoBehaviour' class...
    float thisMeleeForce;
    float thisDamage;
    GameObject thisEnemy;

    //using a contstructor allows us to pass public variables from EnemyBase to this class
    public EnemyMeleeObserver(GameObject thisGameObject, float meleeForce, float damage){
        thisMeleeForce = meleeForce;
        thisEnemy = thisGameObject;
        thisDamage = damage;
    }


    public override Ray CreateRay() {
        //Ray ray = Camera.main.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0));
        return new Ray(thisEnemy.transform.position, thisEnemy.transform.forward);
    }

    public override void TargetSpotted() {
        Debug.Log("Enemy Melee Hit!");
    }

    //below method added by Alder 5/10/22 to implement melee force
    public override void ApplyAttackForce(Collider target){
        //player = GameObject.Find("Player");
        target.attachedRigidbody.AddForce(thisEnemy.transform.forward * thisMeleeForce, ForceMode.Impulse);
        PlayerController scriptInstance = target.GetComponent<PlayerController>();
        scriptInstance.ApplyDamage(thisDamage);
    }
}