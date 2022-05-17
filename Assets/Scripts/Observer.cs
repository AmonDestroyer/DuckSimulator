using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Observer
{
    public string targetTag;
    public float targetRange;
    public Transform sourceTransform;
    public List<Collider> sourceColliders;
    
    private RaycastHit m_RaycastHit;
    private RaycastHit m_DirectRaycastHit;
    private Transform targetTransform;
    private int m_LayerMask = LayerMask.GetMask("Default");

    public abstract Ray CreateRay();
    public abstract void TargetSpotted();
    //Below line added by Alder on 5/10 to implement melee force to enemies
    public abstract void ApplyAttackForce(Collider target);
    
    public void CollisionCheck() {
        foreach(Collider targetCollider in sourceColliders) {
            Debug.Log($"Collider1: {targetCollider.tag}: {targetCollider.CompareTag(targetTag)}");
            if(targetCollider.CompareTag(targetTag)) {
                Debug.Log($"Collider2: {targetCollider.tag}: {targetCollider.CompareTag(targetTag)}");
                targetTransform = targetCollider.transform;
                if(DirectRayCheck()) {
                    Debug.Log("Collision Check Successful!");
                    TargetSpotted();
                    ApplyAttackForce(targetCollider);
                }
            }
        }
    }

    public bool RayCheck() { // helper function for if stuff is hit using modified methods of ray creation
            Ray ray = CreateRay();
            if(Physics.Raycast(ray, out m_RaycastHit, targetRange, m_LayerMask)) {
                targetTransform = m_RaycastHit.collider.transform;
                Debug.Log($"{targetTransform.tag}");
                
                if(DirectRayCheck()) {
                    Debug.Log($"Hit: {m_RaycastHit.collider.transform}, {m_DirectRaycastHit.distance}");
                    return true;
                }
            }
        return false;
    }
    public bool DirectRayCheck() { //checks between two objects directly
        Vector3 direction = targetTransform.position - sourceTransform.position;
        Ray ray = new Ray(sourceTransform.position, direction);
        if(Physics.Raycast(ray, out m_DirectRaycastHit, targetRange, m_LayerMask))
        {
            //Debug.Log($"DirectRayCheck: {targetTransform.tag}");
            //Debug.Log($"DirectRayCheck: {m_DirectRaycastHit.collider.tag}");
            //Debug.Log($"DirectRayCheck: {m_DirectRaycastHit.collider.CompareTag(targetTag)}");
            if(m_DirectRaycastHit.collider.CompareTag(targetTag) && m_DirectRaycastHit.distance <= targetRange)
            {
                return true;
            }
        }
        return false;
    }
}
