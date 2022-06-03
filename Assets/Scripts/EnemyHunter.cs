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
    private Vector3 m_PrevPlayerTransform;
    private bool m_ReachedPartner = true;
    //private int m_LayerMask;

    // Update is called once per frame

    void Start() {
        base.Start();
        ActivateIdle();
        m_LaserRenderer = GetComponentInChildren(typeof(LaserScript)) as LaserScript;
        if(partner != null) {
            m_ReachedPartner = false;
        }
        attackStaminaCost = 0.8f;
        rechargeStep = 0.01f;
        rechargeDelay = 2.0f;
        m_HunterOrigin = transform.Find("HunterOrigin");
        m_LaserRenderer.origin = m_HunterOrigin;
        m_LaserRenderer.target = player.transform.position;
        approachRadius = approachRadius * 8; // hunters see further
    }
    protected override void FollowAndAttackPlayer()
        {
        if(m_TakenDamage && !m_ReachedPartner) {
            ActivateWalk();
            m_walkActivated = true;
            float step =  enemySpeed * Time.fixedDeltaTime;
            Vector3 distanceVector = partner.transform.position - this.transform.position;
            float rotateStep = enemyLookSpeed * Time.fixedDeltaTime;
            float distanceUnrooted = Vector3.Dot(distanceVector, distanceVector);
            RotateTowardsPartner(rotateStep);
            this.transform.position = Vector3.MoveTowards(this.transform.position, partner.transform.position, step);
            if(distanceUnrooted < stopRadius * 5) {
                m_ReachedPartner = true;
            }
        } else {
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
                                m_LaserRenderer.target = player.transform.position;
                                Debug.Log("Hit player transform!");
                                m_LaserRenderer.laserMaxLength = approachRadius;
                                m_LaserRenderer.triggerLaser = true;
                                m_ShotTimer += Time.fixedDeltaTime;
                                Debug.Log($"ShotTimer = {m_ShotTimer}");
                                if(m_ShotTimer > 3.0f) { // about to shoot!
                                    m_LaserRenderer.ChangeColor(Color.red);
                                }
                                if(m_ShotTimer > 3.7f) { // shoot at position after short pause!!
                                    m_UpdateRay = false;
                                    m_PrevPlayerTransform = player.transform.position;
                                }
                            } else { // no longer spotted player
                                m_LaserRenderer.triggerLaser = false;
                                m_UpdateRay = true;
                                m_ShotTimer = 0.0f;
                                m_LaserRenderer.ChangeColor(Color.blue);
                            }
                        }
                    }
                    if(!m_UpdateRay) {
                        Vector3 lastSeen = m_PrevPlayerTransform - m_HunterOrigin.position;
                        Ray hunterRay = new Ray(m_HunterOrigin.position, lastSeen);
                        RaycastHit hunterRaycastHit;
                        Physics.Raycast(hunterRay, out hunterRaycastHit, approachRadius);
                        m_LaserRenderer.target = m_PrevPlayerTransform;
                        m_ShotTimer += Time.fixedDeltaTime;
                        if(player.transform == hunterRaycastHit.collider.transform && m_ShotTimer > 4.0f) {
                            Attack();
                            m_UpdateRay = true;
                            m_ShotTimer = 0.0f;
                            m_LaserRenderer.triggerLaser = false;
                            m_LaserRenderer.ChangeColor(Color.blue);
                        } else if (m_ShotTimer > 4.0f) {
                            this.stamina -= attackStaminaCost;
                            attacking = true;
                            ActivateIdle();
                            m_UpdateRay = true;
                            m_ShotTimer = 0.0f;
                            m_LaserRenderer.triggerLaser = false;
                            m_LaserRenderer.ChangeColor(Color.blue);
                        }
                        
                    }
                }
                
            } else if (m_HuntStarted) {
                m_UpdateRay = true;
                m_ShotTimer = 0.0f;
                m_LaserRenderer.triggerLaser = false;
                m_LaserRenderer.ChangeColor(Color.blue);
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
        }
    }

        protected void RotateTowardsPartner(float step) {
         Vector3 targetDirection = (partner.transform.position - this.transform.position);
         Vector3 newDirection = Vector3.RotateTowards(this.transform.forward, targetDirection, step, 0.0f);
         newDirection.y = 0; //this prevents enemy from flipping around because they want to loop up/down
         this.transform.rotation = Quaternion.LookRotation(newDirection);

    }

    protected override void Attack() {
        attacking = true;
        ++countAttacks;
        this.stamina -= attackStaminaCost;
        Debug.Log("ENEMY Sniped!!");
        Debug.Log("TEST: Stamina value = " + this.stamina + ", attackStaminaCost = " + attackStaminaCost + " # of attacks = " + countAttacks);
        PlayerController playerScript = player.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
        playerScript.ApplyDamage(damage);
    }

    public override void ApplyDamage(float damage) {
        m_ResetVelocity = true;
        health -= damage;
        // sniping gets reset if they get hit
        m_UpdateRay = true;
        m_ShotTimer = 0.0f;
        m_LaserRenderer.triggerLaser = false;
        m_LaserRenderer.ChangeColor(Color.blue);
        FindObjectOfType<AudioManager>().Play("EnemyHurt");
        FindObjectOfType<AudioManager>().ChangePitch("EnemyHurt", Random.Range(0.9f, 1.5f));
        if(!m_TakenDamage) { // will cancel attack and run away
            if(partner != null) {
                partnerScript.approachRadius = approachRadius * 8; // call in a partner
            }
            approachRadius = approachRadius * 2; // if the enemy has been hit, it will not stop pursuing the player; IT WANTS BLOOD
            m_TakenDamage = true;
        }
    }
};
