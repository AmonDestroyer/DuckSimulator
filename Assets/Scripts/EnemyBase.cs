using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float enemySpeed = 3.0f;
    public float enemyLookSpeed = 0.75f;
    public float approachRadius = 100.0f;
    public float stopRadius = 1.5f; // want stop radius to be less than attack radius

    public float health = 1.0f;
  

    // SECTION FOR ENEMY MELEE
    public float attackRadius = 10.0f; //NOTE: if the enemy has a melee, its range will also be affected
    // by the size of the enemyMeleeScope. 'attackRadius' determines the radius in which a enemy 'tries'
    // to attack the player, not the radius in which it will do damage. 
    public float damage = 0.05f;
    public float meleeForce = 15.0f;
    private MeleeScope enemyMeleeScope;
    private EnemyMeleeObserver enemyMeleeObserver;

    //SECTION FOR ENEMY STAMINA
    EnemyStaminaCharger enemyStaminaCharger;
    public float stamina = 1.0f;
    public float rechargeDelay = 0.3f;
    public float initialRechargeDelay = 1.0f;
    public float rechargeStep = 0.5f;
    public float attackStaminaCost = 0.9f;
    public bool attacking = false;
    private int countAttacks = 0;
    


    GameObject player;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.Find("Player");

        enemyMeleeObserver = new EnemyMeleeObserver(gameObject, meleeForce, damage);
        enemyMeleeScope = GetComponentInChildren<MeleeScope>();
        enemyMeleeObserver.targetRange = attackRadius;
        enemyMeleeObserver.targetTag = "Player";
        enemyMeleeObserver.sourceTransform = GetComponent<Transform>();


        enemyStaminaCharger = new EnemyStaminaCharger(gameObject, rechargeStep, rechargeDelay, initialRechargeDelay);
        StartCoroutine(enemyStaminaCharger.startRegen());
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {   
        if(health > 0.0f){
            FollowAndAttackPlayer();
        } else{
            Dead();
        }
    }

    protected void FollowAndAttackPlayer()
    {
        Vector3 distanceVector = player.transform.position - this.transform.position;
        float distanceUnrooted = Vector3.Dot(distanceVector, distanceVector);
        // I omit the square root part of the distance equation to reduce overhead
        if(distanceUnrooted < approachRadius && distanceUnrooted > stopRadius){ 
            float step =  enemySpeed * Time.fixedDeltaTime;
            float rotateStep = enemyLookSpeed * Time.fixedDeltaTime;
            RotateTowardsPlayer(rotateStep); // used seperate method here in case we want
            // ranged enemies that don't move and just rotate at some point
            this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);

            

        }
        // ENEMY ATTACK GOES HERE
        if(distanceUnrooted < attackRadius && this.stamina > attackStaminaCost){    
            Debug.Log("ENEMY ATTACKED!");  
            Debug.Log("TEST: Stamina value = " + this.stamina + ", attackStaminaCost = " + attackStaminaCost + " # of attacks = " + countAttacks);         
            Attack();
            attacking = true;
            this.stamina -= attackStaminaCost;
            ++countAttacks;
        }
    }

    protected void RotateTowardsPlayer(float step){
         Vector3 targetDirection = (player.transform.position - this.transform.position);
         Vector3 newDirection = Vector3.RotateTowards(this.transform.forward, targetDirection, step, 0.0f);
         this.transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void Attack(){
        enemyMeleeObserver.sourceColliders = enemyMeleeScope.TriggerList;
        enemyMeleeObserver.CollisionCheck();
    }

    public void ApplyDamage(float damage){
        health -= damage;
    }

    void Dead(){ // call this when health < 0
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
    }

}
