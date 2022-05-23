using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public 
    private Rigidbody m_rigid;
    
    void Awake() {
        m_rigid = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other) {
        target = other.gameObject;
        if(target.CompareTag("Enemy")) {
            Hit(target);
        }
        Destroy(this.gameObject);
    }

    void Hit(GameObject target) {
        
    }
};
