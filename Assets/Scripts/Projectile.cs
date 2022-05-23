using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public string targetTag; // what to attack when it hits
    public float despawnTime = 2.0f; // time till despawn after hitting non-target object
    public float longDespawn = 15.0f; // safety check; if still in existence after this period, delete
    public GameObject source;
    private Rigidbody m_rigid; // rigidbody of projectile
    private float m_despawnTimer; // records time after hitting something
    private float m_longDespawnTimer; // records time since initialization
    private bool m_timerStart; // whether timer should be started
    private bool m_onGround; // whether object is on ground
    private Quaternion m_initialRotation; // initial rotation of object


    void Awake() {
        // setting initial values
        m_timerStart = false;
        m_onGround = false;
        m_despawnTimer = 0.0f;
        m_longDespawnTimer = 0.0f;
        m_rigid = GetComponent<Rigidbody>();
        m_initialRotation = transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        m_longDespawnTimer += Time.deltaTime; // always increment
        if(!m_onGround) { // don't do cool rotation thingy if on ground
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
        GameObject target = other.gameObject;
        Debug.Log($"{target}");
        if(target.CompareTag(targetTag) && (m_despawnTimer < 0.2f)) { // only works if enemy is hit FIRST (or within small range of error)!
            Hit(target); // do a damage effect 
            Destroy(this.gameObject); // then delete projectile
        }
        if (target.CompareTag("Ground")) { // checks for ground positioning
            m_onGround = true;
        } else {
            m_onGround = false;
        }
        //m_rigid.velocity = Vector3.zero;
        if(target != source) {
            m_timerStart = true;
        }
    }

    void Hit(GameObject target) { // currently does hit; will do more later!
        Debug.Log($"Feather hit {targetTag}!");
    }
};
