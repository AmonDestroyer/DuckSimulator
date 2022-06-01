using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeObserver : Observer
{   
    float meleeForce; // knockback dealt to target
    float damage; // damage dealt to target
    GameObject self; // object of origin


    public MeleeObserver(GameObject thisGameObject, float meleeForce, float damage){
        this.meleeForce = meleeForce;
        this.self = thisGameObject;
        this.damage = damage;
    }

    public override Ray CreateRay() {
        return new Ray(this.self.transform.position, this.self.transform.forward);
    }

    public override void TargetSpotted() {
        Debug.Log($"{targetTag} Melee Hit!");
    }

    //below method added by Alder 5/10/22 to implement melee force
    public override void ApplyAttackForce(Collider target){
        target.attachedRigidbody.AddForce(self.transform.forward * this.meleeForce, ForceMode.Impulse);
        if(target.tag == "Player") {
            GameObject player = target.gameObject;
            PlayerController player_controller = player.GetComponent<PlayerController>();
            player_controller.ApplyDamage(damage);
        }
        if(target.tag == "Enemy") {
            GameObject enemy = target.gameObject;
            EnemyBase enemy_base = enemy.GetComponent<EnemyBase>();
            enemy_base.ApplyDamage(damage);
        }
        //EnemyBase scriptInstance = target.GetComponent<EnemyBase>();
        //scriptInstance.ApplyDamage(this.damage);
    }
};
