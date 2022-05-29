using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public PlayerController player;
    // Start is called before the first frame update
    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren(typeof(PlayerController)) as PlayerController;
    }
    void Start()
    {
        player.spawnPoint = transform;
        player.Respawn();
    }

    // Update is called once per frame

}
