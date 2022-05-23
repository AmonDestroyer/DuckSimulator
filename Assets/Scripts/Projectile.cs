using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public string targetTag;
    public float despawnTime = 2.0f;
    private Rigidbody m_rigid;
    private float m_despawnTimer;
    private bool m_timerStart;

    void Awake() {
        m_timerStart = false;
        m_despawnTimer = 0.0f;
        m_rigid = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if(m_timerStart) {
            m_despawnTimer += Time.deltaTime;
        }
        if(m_despawnTimer >= despawnTime) {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision other) {
        GameObject target = other.gameObject;
        if(target.CompareTag(targetTag) && !(m_timerStart)) { // only works if enemy is hit FIRST!
            Hit(target);
            Destroy(this.gameObject);
        }
        m_timerStart = true;
    }

    void Hit(GameObject target) {
        Debug.Log($"Feather hit {targetTag}!");
    }
};
