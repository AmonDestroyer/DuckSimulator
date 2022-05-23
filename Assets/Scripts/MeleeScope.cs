using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeScope : MonoBehaviour
{
    // https://answers.unity.com/questions/638319/getting-a-list-of-colliders-inside-a-trigger.html
    private List<Collider> TriggerList;

    void Awake() {
        TriggerList = new List<Collider>();
    }

    public List<Collider> getTriggerList() { // prevents null access
        RemoveNulls();
        return TriggerList;
    }

    void OnTriggerEnter(Collider other) { // if in melee scope, add to list
        if(!TriggerList.Contains(other))
        {
            TriggerList.Add(other);
        }
    }

    void OnTriggerExit(Collider other) { // if exits melee scope, remove from list
        if(TriggerList.Contains(other))
        {
            TriggerList.Remove(other);
        }
    }

    private void RemoveNulls() {
        TriggerList.RemoveAll(x => x == null);
    }
}
