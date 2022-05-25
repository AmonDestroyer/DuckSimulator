using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable // WIP
{
    // VARIABLES
    // health
    public float health; // amount of damage killable can take before dying
    // movement
    public float speed; // normal movement speed of killable
    public float speed_fast; // speed for faster movement (sprint in players; retreat/fear in enemies)
    public float speed_slow; // speed for slower movement (crouch for players; slowed/attacking(?) for enemies)
    // stamina
    float rechargeStep; // amount stamina recharges every iteration
    float stepDelay; // delay until stamina charges up more
    float initialStepDelay; // delay until stamina starts to recharge
    // damage


};
