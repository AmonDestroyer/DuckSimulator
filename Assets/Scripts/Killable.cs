using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable
{
    // VARIABLES
    // health
    public float health; // life of killable
    // movement
    public float speed; // normal movement speed of killable
    public float speed_fast; // speed for faster movement (sprint in players; retreat/fear in enemies)
    public float speed_slow; // speed for slower movement (crouch for players; slowed/attacking(?) for enemies)
    // stamina
    float rechargeStep;
    float stepDelay; 
    float initialStepDelay;
    // damage


};
