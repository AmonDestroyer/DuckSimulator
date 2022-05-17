using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float enemySpeed = 3.0f;
    public float enemyLookSpeed = 0.75f;
    public float approachRadius = 100.0f;

    public float health = 1.0f;
  

    // SECTION FOR ENEMY MELEE
    public float attackRadius = 10.0f; //NOTE: if the enemy has a melee, its range will also be affected
    // by the size of the enemyMeleeScope. 'attackRadius' determines the radius in which a enemy 'tries'
    // to attack the player, not the radius in which it will do damage. 
    public float damage = 0.05f;
    public float meleeForce = 5.0f;
    public float attackDelay = 0.7f;
    public MeleeScope enemyMeleeScope;
    private EnemyMeleeObserver enemyMeleeObserver;
    


    GameObject player;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.Find("Player");

        enemyMeleeObserver = new EnemyMeleeObserver(gameObject, meleeForce, damage);
        enemyMeleeObserver.targetRange = attackRadius;
        enemyMeleeObserver.targetTag = "Player";
        enemyMeleeObserver.sourceTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        FollowAndAttackPlayer();
    }

    protected void FollowAndAttackPlayer()
    {
        Vector3 distanceVector = player.transform.position - this.transform.position;
        float distanceUnrooted = Vector3.Dot(distanceVector, distanceVector);
        // I omit the square root part of the distance equation to reduce overhead
        if(distanceUnrooted < approachRadius){ 
            float step =  enemySpeed * Time.fixedDeltaTime;
            float rotateStep = enemyLookSpeed * Time.fixedDeltaTime;
            RotateTowardsPlayer(rotateStep); // used seperate method here in case we want
            // ranged enemies that don't move and just rotate at some point
            this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);

            // ENEMY ATTACK GOES HERE
            if(distanceUnrooted < attackRadius){
                Attack();
            }

        }
    }

    protected void RotateTowardsPlayer(float step){
         Vector3 targetDirection = (player.transform.position - this.transform.position);
         Vector3 newDirection = Vector3.RotateTowards(this.transform.forward, targetDirection, step, 0.0f);
         this.transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void Attack(){
        // StartCoroutine(makeWait());
        enemyMeleeObserver.sourceColliders = enemyMeleeScope.TriggerList;
        enemyMeleeObserver.CollisionCheck();
    }

    /* IEnumerator attackWithDelay(){
        yield return new WaitForSeconds(attackDelay);
        Attack();
    } */

}
