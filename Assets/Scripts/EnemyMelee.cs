using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyBase
{
    // Start is called before the first frame update
    private float m_StaminaTimer = 0.0f;
    private bool m_StaminaTimerFull = false;
    private bool m_HuntStarted = false;
    private float enemySpeedBase;

    // Update is called once per frame
        void Start() {
            base.Start();
            enemySpeedBase = enemySpeed;
        }
        protected override void FollowAndAttackPlayer()
        { 
        Vector3 distanceVector = player.transform.position - this.transform.position;
        float distanceUnrooted = Vector3.Dot(distanceVector, distanceVector);
        // I omit the square root part of the distance equation to reduce overhead
        if(distanceUnrooted < approachRadius && distanceUnrooted > stopRadius){ 
            if(!m_HuntStarted) {
                approachRadius = approachRadius * 4f; // once player has been spotted, it gets HARD to run away
                m_HuntStarted = true;
            } 
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
            if(m_HuntStarted) {
                approachRadius = approachRadius / 4f; // if player DOES get away, approachRadius goes back to normal
                m_HuntStarted = false;
            } 
            ActivateIdle();
            Debug.Log("Activating Idle");
            enemySpeed = enemySpeedBase;
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
            m_StaminaTimer = 0.0f;
        } else if (distanceUnrooted > attackRadius && distanceUnrooted < approachRadius && stamina == 1.0f){
            if(!m_StaminaTimerFull) {
                m_StaminaTimer += Time.fixedDeltaTime;
                if(debug) 
                    Debug.Log($"{m_StaminaTimer}");
            }
        }
        if(m_StaminaTimerFull) {
            m_StaminaTimer -= Time.fixedDeltaTime;
            if(debug) 
                    Debug.Log($"{m_StaminaTimer}");
            if(m_StaminaTimer <= 0.0f) {
                stamina = 0.0f;
                m_StaminaTimerFull = false;
                enemySpeed = enemySpeedBase;
            }
        }
        if(distanceUnrooted > attackRadius && distanceUnrooted < approachRadius && m_StaminaTimer > 4.0f) {
            if(!m_StaminaTimerFull) {
                int random = 0;//Random.Range(0,2);
                if(random == 0) { // chase action
                    enemySpeed = enemySpeed * 2;
                }
                m_StaminaTimerFull = true;
            }
        }
    }
}
