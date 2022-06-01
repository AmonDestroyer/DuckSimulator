using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float enemySpeed = 4.2f;
    public float enemyLookSpeed = 1.5f;
    public float approachRadius = 175.0f;
    public float stopRadius = 0f; // want stop radius to be less than attack radius

    public float health = 1.0f;
  

    // SECTION FOR ENEMY MELEE
    public float attackRadius = 10.0f; //NOTE: if the enemy has a melee, its range will also be affected
    // by the size of the enemyMeleeScope. 'attackRadius' determines the radius in which a enemy 'tries'
    // to attack the player, not the radius in which it will do damage. 
    public float damage = 0.05f;
    public float meleeForce = 15.0f;
    private MeleeScope enemyMeleeScope;
    private MeleeObserver enemyMeleeObserver;

    //SECTION FOR ENEMY STAMINA
    EnemyStaminaCharger enemyStaminaCharger;
    public float stamina = 1.0f;
    public float rechargeDelay = 0.3f;
    public float initialRechargeDelay = 1.0f;
    public float rechargeStep = 0.5f;
    public float attackStaminaCost = 0.9f;
    public bool attacking = false;
    private int countAttacks = 0;

    //SECTION FOR ENEMY ANIMATIONS
    public int startPosition = 3; // sets the enemies starting position, 3 = Sitting, 5 = Walk
    private Animator m_ani; // see readMe in simple modular human package for more options
    bool m_walkActivated = false;
    bool m_lookAlternator = true;
    bool m_isLooking = false; // USE THIS TO PREVENT CALLING MULTIPLE COROUTINES!
    


    GameObject player;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.Find("Player");

        enemyMeleeObserver = new MeleeObserver(gameObject, meleeForce, damage);
        enemyMeleeScope = GetComponentInChildren<MeleeScope>();
        m_ani = GetComponentInChildren<Animator>();
        SetStartingPosition();

        enemyMeleeObserver.targetRange = attackRadius;
        enemyMeleeObserver.targetTag = "Player";
        enemyMeleeObserver.sourceTransform = GetComponent<Transform>();


        enemyStaminaCharger = new EnemyStaminaCharger(gameObject, rechargeStep, rechargeDelay, initialRechargeDelay);
        StartCoroutine(enemyStaminaCharger.startRegen());

        // Start the enemies looking around 
        StartCoroutine("LookingAroundCoroutine");
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

            
            if(m_isLooking){
                StopCoroutine("LookingAroundCoroutine"); // double StopCoroutine to work around bug where multiple StartCoroutines
                StopCoroutine("LookingAroundCoroutine"); // are called creating multiple instances 

                m_isLooking = false;
            }

            ActivateWalk();
            m_walkActivated = true;

            float step =  enemySpeed * Time.fixedDeltaTime;
            float rotateStep = enemyLookSpeed * Time.fixedDeltaTime;
            RotateTowardsPlayer(rotateStep); // used seperate method here in case we want
            // ranged enemies that don't move and just rotate at some point
            this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);

            

        } else if(m_walkActivated){
            ActivateIdle();
            Debug.Log("Activating Idle");
            m_walkActivated = false;
            if(!m_isLooking){
                StopCoroutine("LookingAroundCoroutine"); // double StopCoroutine to work around bug where multiple StartCoroutines
                StopCoroutine("LookingAroundCoroutine"); // are called creating multiple instances 
                
                StartCoroutine("LookingAroundCoroutine");
                m_isLooking = true;
            }
            
        }
        // ENEMY ATTACK GOES HERE
        if(distanceUnrooted < attackRadius && this.stamina > attackStaminaCost){    
            Debug.Log("ENEMY ATTACKED!");  
            Debug.Log("TEST: Stamina value = " + this.stamina + ", attackStaminaCost = " + attackStaminaCost + " # of attacks = " + countAttacks);         
            Attack();
            ActivateHit();
            attacking = true;
            this.stamina -= attackStaminaCost;
            ++countAttacks;
        }
    }

    protected void RotateTowardsPlayer(float step){
        
         Vector3 targetDirection = (player.transform.position - this.transform.position);
         Vector3 newDirection = Vector3.RotateTowards(this.transform.forward, targetDirection, step, 0.0f);
         newDirection.y = 0; //this prevents enemy from flipping around because they want to loop up/down
         this.transform.rotation = Quaternion.LookRotation(newDirection);
         
    }

    void Attack(){
        enemyMeleeObserver.sourceColliders = enemyMeleeScope.getTriggerList();
        enemyMeleeObserver.CollisionCheck();
    }

    public void ApplyDamage(float damage){
        health -= damage;
    }

    void Dead(){ // call this when health < 0
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(Vector3.up * 1f, ForceMode.Impulse);
        ActivateIdle(); // otherwise player keeps walking in air after death
        if(this.transform.position.y > 420){ //deactivate enemy when they get high to lower overhead
            this.gameObject.SetActive(false);
        }
    }

    void ActivateWalk(){
        m_ani.SetInteger("arms", 1); // this sets the arms animation to setting 1 - walk
        m_ani.SetInteger("legs", 1); // see the readMe in the simple modular human package
    }

    void ActivateIdle(){
        m_ani.SetInteger("arms", 5);
        m_ani.SetInteger("legs", 5);
    }

    void ActivateHit(){
        m_ani.SetInteger("arms", 14);
    }

    void SetStartingPosition(){
        m_ani.SetInteger("arms", startPosition);
        m_ani.SetInteger("legs", startPosition);
    }

    IEnumerator LookingAroundCoroutine(){
        Debug.Log("Called Looking around coroutine");

        yield return new WaitForSeconds(Random.Range(5, 10));


        if(m_lookAlternator){ // need to alternate between animations to keep look animation going
            m_ani.SetInteger("arms", 9);
            m_lookAlternator = false;
        } else{
            m_ani.SetInteger("arms", 10);
            m_lookAlternator = true;
        }

        if(m_isLooking){
            StopCoroutine("LookingAroundCoroutine"); // double StopCoroutine to work around bug where multiple StartCoroutines
            StopCoroutine("LookingAroundCoroutine"); // are called creating multiple instances 
            m_isLooking = false;
        }

        if(!m_isLooking){
            StartCoroutine("LookingAroundCoroutine");
            m_isLooking = true;
        }
        

    }

}
