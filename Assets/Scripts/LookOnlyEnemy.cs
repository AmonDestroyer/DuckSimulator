using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookOnlyEnemy : EnemyBase
{
    public float localEnemyLookSpeed = 0.42f;
 
    // Update is called once per frame
    public override void FixedUpdate()
    {
     RotateTowardsPlayer(localEnemyLookSpeed * Time.fixedDeltaTime);   
    }
    protected override void FollowAndAttackPlayer() {}
    protected override void Attack() {}

    public override void ApplyDamage(float damage){
        Debug.Log($"{damage}");
    }

}
