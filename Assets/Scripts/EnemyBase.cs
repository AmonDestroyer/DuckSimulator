using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public float enemySpeed = 8.4f;
    public float enemyLookSpeed = 1.8f;
    public float approachRadius = 175.0f;
    public float stopRadius = 10f; // want stop radius to be less than attack radius
    public bool debug = false;
    public float health = 1.0f;
    public GameObject partner; // another enemy; if one gets attacked, partners can be dragged into the fight
    
    // for giving enemies more flavorful stats
    protected EnemyBase partnerScript;
    protected bool m_ResetVelocity = false;
    protected bool deathSoundPlayed = false;
    public bool overrideScale = false;
    protected float m_Scale;
    protected bool isBoss = false;
    public struct StatCaps { // yes, I know this is overkill rn; might make stuff easier later (after the final build but still)

        public (float, float) healthCaps; // more health if bigger; less if smaller
        public (float, float) speedCaps; // little guys go faster; big guys go slower
        public (float, float) damageCaps; // more damage if bigger; less if smaller
        public (float, float) forceCaps; // more knockback if bigger; less if smaller;
        public (float, float) lookSpeedCaps; // turn slower if bigger; turn faster if smaller
        public (float, float) rechargeStepCaps; // smaller enemies make more frequent attacks; bigger enemies make fewer

        public StatCaps((float, float) health, (float, float) speed, (float, float) damage,
                        (float, float) force, (float, float) lookSpeed, (float, float) rechargeStep) {
            healthCaps = health;
            speedCaps = speed;
            damageCaps = damage;
            forceCaps = force;
            lookSpeedCaps = lookSpeed;
            rechargeStepCaps = rechargeStep;
        }
    };

    protected StatCaps EnemyStatCaps = new StatCaps((0.6f, 10f), (12f, 4f), (0.02f, 0.2f), (5f, 40f), (2.1f, 1.3f), (.5f, .9f));
    // SECTION FOR ENEMY MELEE
    public float attackRadius = 10.0f; //NOTE: if the enemy has a melee, its range will also be affected
    // by the size of the enemyMeleeScope. 'attackRadius' determines the radius in which a enemy 'tries'
    // to attack the player, not the radius in which it will do damage.
    public float damage = 0.05f;
    public float meleeForce = 15.0f;
    protected MeleeScope enemyMeleeScope;
    protected MeleeObserver enemyMeleeObserver;
    protected bool m_TakenDamage = false;
    //SECTION FOR ENEMY STAMINA
    EnemyStaminaCharger enemyStaminaCharger;
    public float stamina = 1.0f;
    public float rechargeDelay = 0.3f;
    public float initialRechargeDelay = 1.0f;
    public float rechargeStep = 0.5f;
    public float attackStaminaCost = 0.9f;
    public bool attacking = false;
    protected int countAttacks = 0;



    //SECTION FOR ENEMY ANIMATIONS
    public int startPosition = 3; // sets the enemies starting position, 3 = Sitting, 5 = Walk
    protected Animator m_ani; // see readMe in simple modular human package for more options
    protected bool m_walkActivated = false;
    protected bool m_lookAlternator = true;
    protected bool m_isLooking = false; // USE THIS TO PREVENT CALLING MULTIPLE COROUTINES!



    protected GameObject player;

    // Start is called before the first frame update
    public virtual void Start()
    {
        if(partner != null) {
            partnerScript = partner.GetComponentInChildren<EnemyBase>();
        }
        m_Scale = transform.localScale.x;
        if(m_Scale >= 3.0f) {
            isBoss = true;
        }
        if(!overrideScale) {
            InterpolateStats();
        }
        stopRadius = stopRadius * m_Scale;
        attackRadius = attackRadius * m_Scale;
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
        } else {
            Dead();
        }
    }

    protected abstract void FollowAndAttackPlayer();

    protected void RotateTowardsPlayer(float step) {
         Vector3 targetDirection = (player.transform.position - this.transform.position);
         Vector3 newDirection = Vector3.RotateTowards(this.transform.forward, targetDirection, step, 0.0f);
         newDirection.y = 0; //this prevents enemy from flipping around because they want to loop up/down
         this.transform.rotation = Quaternion.LookRotation(newDirection);

    }

    protected abstract void Attack();

    public abstract void ApplyDamage(float damage);

    void Dead(){ // call this when health < 0
        if (!deathSoundPlayed)
        {
          FindObjectOfType<AudioManager>().Play("EnemyDeath");
          deathSoundPlayed = true;
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(Vector3.up * 1f, ForceMode.Impulse);
        ActivateIdle(); // otherwise player keeps walking in air after death
        if(this.transform.position.y > 420){ //deactivate enemy when they get high to lower overhead
            this.gameObject.SetActive(false);
        }
    }

    void InterpolateStats() {
        health = Mathf.Lerp(EnemyStatCaps.healthCaps.Item1, EnemyStatCaps.healthCaps.Item2, m_Scale/5);
        enemySpeed = Mathf.Lerp(EnemyStatCaps.speedCaps.Item1, EnemyStatCaps.speedCaps.Item2, m_Scale/5);
        damage = Mathf.Lerp(EnemyStatCaps.damageCaps.Item1, EnemyStatCaps.damageCaps.Item2, m_Scale/5);
        meleeForce = Mathf.Lerp(EnemyStatCaps.forceCaps.Item1, EnemyStatCaps.forceCaps.Item2, m_Scale/5);
        enemyLookSpeed = Mathf.Lerp(EnemyStatCaps.lookSpeedCaps.Item1, EnemyStatCaps.lookSpeedCaps.Item2, m_Scale/5);
        rechargeStep = Mathf.Lerp(EnemyStatCaps.rechargeStepCaps.Item1, EnemyStatCaps.rechargeStepCaps.Item2, m_Scale/5);
    }

    protected void ActivateWalk(){
        m_ani.SetInteger("arms", 1); // this sets the arms animation to setting 1 - walk
        m_ani.SetInteger("legs", 1); // see the readMe in the simple modular human package
    }

    protected void ActivateIdle(){
        m_ani.SetInteger("arms", 5);
        m_ani.SetInteger("legs", 5);
    }

    protected void ActivateHit(){
        m_ani.SetInteger("arms", 14);
    }

    void SetStartingPosition() {
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
