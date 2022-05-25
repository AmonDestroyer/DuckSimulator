using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Alder's StaminaCharger class for recharging player's and enemies' stamina
// over time. To use, create an instance of this class in EnemyBase or PlayerController.cs
// and then call startRegen() in Start() or Awake()
// Code inspiration comes from this youtube video:
// https://www.youtube.com/watch?v=sUvwKH7qyQQ
public class EnemyStaminaCharger
{   
    EnemyBase enemyScript;
    float rechargeStep;
    float stepDelay; 
    float initialStepDelay;
    public EnemyStaminaCharger(GameObject thisEnemy, float rechargeStep, float stepDelay, float initialStepDelay){
        enemyScript = thisEnemy.GetComponent<EnemyBase>();
        this.rechargeStep = rechargeStep;
        this.stepDelay = stepDelay;
        this.initialStepDelay = initialStepDelay;
    }

    public IEnumerator startRegen(){
        while(enemyScript.health > 0.0f){
            if(enemyScript.stamina < 1.0f){
                if(enemyScript.attacking){
                    yield return new WaitForSeconds(initialStepDelay);
                    enemyScript.attacking = false;
                    enemyScript.stamina += rechargeStep;
                    Debug.Log("Increased characterStamina in StaminaCharger! characterStamina = " + enemyScript.stamina);
                } else {
                    enemyScript.stamina += rechargeStep;
                    Debug.Log("Increased characterStamina in StaminaCharger! characterStamina = " + enemyScript.stamina);
                }
                if(enemyScript.stamina > 1.0f){
                    enemyScript.stamina = 1.0f;
                }
            }
            yield return new WaitForSeconds(stepDelay);
        }
    }
};
