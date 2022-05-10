using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float enemySpeed = 3.0f;
    public float enemyLookSpeed = 0.75f;
    public float approachRadius = 100.0f;
    GameObject player;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.Find("Player");

    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        FollowPlayer();
    }

    protected void FollowPlayer()
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
        }
    }

    protected void RotateTowardsPlayer(float step){
         Vector3 targetDirection = player.transform.position - this.transform.position;
         Vector3 newDirection = Vector3.RotateTowards(this.transform.forward, targetDirection, step, 0.0f);
         this.transform.rotation = Quaternion.LookRotation(newDirection);
    }

}
