using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public string targetTag = "Enemy"; // what to attack when it hits
    public float despawnTime = 2.0f; // time till despawn after hitting non-target object
    public float longDespawn = 5.0f; // safety check; if still in existence after this period, delete
    public GameObject target; // temp
    public Vector3 targetPosition;
    public float speed;
    public float damage;
    public float smooth = 1.0f;
    private GameObject player;
    private GameObject source;
    private EnemyBase hit_script;
    private Rigidbody m_rigid; // rigidbody of projectile
    private float m_despawnTimer; // records time after hitting something
    private float m_longDespawnTimer; // records time since initialization
    private bool m_timerStart; // whether timer should be started
    private bool m_onGround; // whether object is on ground
    private Vector3 direction;
    private bool m_FirstUpdate = true;
    private Quaternion m_initialRotation; // initial rotation of object

    void Awake() {
        // setting initial values
        m_timerStart = false;
        m_onGround = false;
        m_despawnTimer = 0.0f;
        m_longDespawnTimer = 0.0f;
        m_rigid = GetComponent<Rigidbody>();
        m_initialRotation = transform.rotation;
        FindObjectOfType<AudioManager>().Play("BowShoot");
        player = GameObject.FindGameObjectWithTag("Player");
        source = player.transform.Find("ProjectileOrigin").gameObject;
        Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>());
        Physics.IgnoreCollision(GetComponent<Collider>(), source.GetComponent<Collider>());
    }
    // Update is called once per frame
    void FixedUpdate()
    {    
        // m_rigid.transform.Translate(m_rigid.transform.forward * speed * Time.fixedDeltaTime);
        if(m_FirstUpdate) {
            direction = targetPosition - transform.position;
            m_FirstUpdate = false;
        }
        m_longDespawnTimer += Time.fixedDeltaTime; // always increment
        if(!m_onGround) { // don't do cool rotation thingy if on ground
            if(m_longDespawnTimer > 0.3f) {
                m_rigid.AddForce(speed * direction * Time.fixedDeltaTime);
            }
            transform.rotation = Quaternion.LookRotation(m_rigid.velocity) * m_initialRotation;
        }
        if(m_timerStart) { // if enemy not hit, stick around for some amount of time...
            m_despawnTimer += Time.deltaTime;
        }
        if(m_despawnTimer >= despawnTime) { // after brief period, despawn
            Destroy(this.gameObject);
        }
        if(m_longDespawnTimer >= longDespawn) { // if it still exists after too long, delete anyways
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision other) {
        target = other.gameObject;
        if(target.CompareTag(targetTag) && (m_despawnTimer < 0.5f)) { // only works if enemy is hit FIRST (or within small range of error)!
            Hit(); // do a damage effect
            Destroy(this.gameObject); // then delete projectile
        }
        m_onGround = true;
        m_rigid.useGravity = false;
        m_rigid.freezeRotation = true;
        m_rigid.velocity = Vector3.zero;
        m_rigid.isKinematic = false;
        m_rigid.detectCollisions = false;

        //m_rigid.velocity = Vector3.zero;
        if(target != source) {
            m_timerStart = true;
        }
    }

    void Hit() { // currently does hit; will do more later!
        hit_script = target.GetComponent<EnemyBase>();
        hit_script.ApplyDamage(damage);
        FindObjectOfType<AudioManager>().Play("BowHit");
        Debug.Log($"Feather hit {targetTag}!");
    }
};
