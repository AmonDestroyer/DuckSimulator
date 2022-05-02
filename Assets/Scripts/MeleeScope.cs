using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeScope : MonoBehaviour
{
    // https://answers.unity.com/questions/638319/getting-a-list-of-colliders-inside-a-trigger.html
    public List<Collider> TriggerList;

    void Awake() {
        TriggerList = new List<Collider>();
    }

    void OnTriggerEnter(Collider other) {
        if(!TriggerList.Contains(other))
        {
            TriggerList.Add(other);
        }
    }

    void OnTriggerExit(Collider other) {
        if(TriggerList.Contains(other))
        {
            TriggerList.Remove(other);
        }
    }

}
