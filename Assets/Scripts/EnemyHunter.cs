using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHunter : EnemyBase
{
    // Start is called before the first frame update
    private Transform m_HunterOrigin;
    private bool m_HuntStarted = false;
    private float m_ShotTimer = 0.0f;
    private LaserScript m_LaserRenderer;
    private bool m_UpdateRay = true;
    //private int m_LayerMask;

    // Update is called once per frame

    void Start() {
        base.Start();
        ActivateIdle();
        m_LaserRenderer = GetComponentInChildren(typeof(LaserScript)) as LaserScript;
        attackStaminaCost = 0.8f;
        rechargeStep = 0.1f;
        m_HunterOrigin = transform.Find("HunterOrigin");
        approachRadius = approachRadius * 8; // hunters see further
    }
    protected override void FollowAndAttackPlayer()
        {
        Vector3 distanceVector = player.transform.position - this.transform.position;
        float distanceUnrooted = Vector3.Dot(distanceVector, distanceVector);
        // I omit the square root part of the distance equation to reduce overhead
        if(distanceUnrooted < approachRadius) {
            if(!m_HuntStarted) {
                approachRadius = approachRadius * 2f; // once player has been spotted, it gets HARD to run away
                Debug.Log("Hunt started!");
                m_HuntStarted = true;
            } 
            if(m_isLooking){
                StopCoroutine("LookingAroundCoroutine"); // double StopCoroutine to work around bug where multiple StartCoroutines
                StopCoroutine("LookingAroundCoroutine"); // are called creating multiple instances 

                m_isLooking = false;
            }
            
            //ActivateWalk();
            //m_walkActivated = true;
            
            //float step =  enemySpeed * Time.fixedDeltaTime;
            float rotateStep = enemyLookSpeed * Time.fixedDeltaTime;
            RotateTowardsPlayer(rotateStep); // used seperate method here in case we want
            // ranged enemies that don't move and just rotate at some point
            //this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);
            if(this.stamina > attackStaminaCost) {
                Vector3 OriginVector = player.transform.position - m_HunterOrigin.position;
                ActivateHit(); // put those hands in the AIR babyy
                if(m_UpdateRay) {
                    Ray hunterRay = new Ray(m_HunterOrigin.position, OriginVector);
                    RaycastHit hunterRaycastHit;
                    if(Physics.Raycast(hunterRay, out hunterRaycastHit, approachRadius)) {
                        Debug.Log($"Hit {hunterRaycastHit.collider.name}!");
                        if(player.transform == hunterRaycastHit.collider.transform) {
                            Debug.Log("Hit player transform!");
                            m_LaserRenderer.laserMaxLength = approachRadius;
                            m_LaserRenderer.triggerLaser = true;
                            m_ShotTimer += Time.fixedDeltaTime;
                            Debug.Log($"ShotTimer = {m_ShotTimer}");
                            if(m_ShotTimer > 1.5f) { // about to shoot!
                                m_LaserRenderer.ChangeColor(Color.white);
                            }
                            if(m_ShotTimer > 1.9f) { // shoot at position after short pause!!
                                m_UpdateRay = false;
                            }
                        }
                    } else { // no longer spotted player
                        m_LaserRenderer.triggerLaser = false;
                        m_UpdateRay = true;
                        m_ShotTimer = 0.0f;
                        m_LaserRenderer.ChangeColor(Color.red);
                    }
                }
                if(!m_UpdateRay) {
                    m_ShotTimer += Time.fixedDeltaTime;
                    if(m_ShotTimer > 2.0f) {
                        Attack();
                        m_UpdateRay = true;
                        m_ShotTimer = 0.0f;
                        m_LaserRenderer.triggerLaser = false;
                        m_LaserRenderer.ChangeColor(Color.red);
                    }
                }
            }
        } else if (m_HuntStarted) {
            m_UpdateRay = true;
            m_ShotTimer = 0.0f;
            m_LaserRenderer.triggerLaser = false;
            m_LaserRenderer.ChangeColor(Color.red);
            approachRadius = approachRadius / 4f; // if player DOES get away, approachRadius goes back to normal
            m_HuntStarted = false;
            ActivateIdle();
            Debug.Log("Activating Idle");
            m_walkActivated = false;
            if(!m_isLooking) {
                StopCoroutine("LookingAroundCoroutine"); // double StopCoroutine to work around bug where multiple StartCoroutines
                StopCoroutine("LookingAroundCoroutine"); // are called creating multiple instances 
                
                StartCoroutine("LookingAroundCoroutine");
                m_isLooking = true;
            }
            
        }
        // ENEMY ATTACK GOES HERE - for hunters, approach radius IS attack radius
    }

    protected override void Attack() {
        Debug.Log("ENEMY Sniped!!");
        Debug.Log("TEST: Stamina value = " + this.stamina + ", attackStaminaCost = " + attackStaminaCost + " # of attacks = " + countAttacks);
        attacking = true;
        ++countAttacks;
        this.stamina -= attackStaminaCost;
        PlayerController playerScript = player.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
        playerScript.ApplyDamage(damage);
    }

    public override void ApplyDamage(float damage) {
        m_ResetVelocity = true;
        health -= damage;
        FindObjectOfType<AudioManager>().Play("EnemyHurt");
        FindObjectOfType<AudioManager>().ChangePitch("EnemyHurt", Random.Range(0.9f, 1.5f));
        if(!m_TakenDamage) { // will cancel attack and run away
            approachRadius = approachRadius * 2; // if the enemy has been hit, it will not stop pursuing the player; IT WANTS BLOOD
            m_TakenDamage = true;
        }
    }
};
