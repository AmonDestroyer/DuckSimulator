using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Pond : MonoBehaviour
{
    public GameObject sampleInstance;
    /*
    This script is attached to the player and used to manage events of the
    scenery with the player. This will manage secret areas, scene change, etc.
    */

    public bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
      sampleInstance = gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other){
      //Loads the appropraite scene when the player touches the portal
      switch(other.gameObject.name)
      {
          case "EndTrigger":
            SceneManager.LoadScene("HomeBaseScene");
            Destroy(sampleInstance);
            break;
          default:
            if (debug)
              Debug.Log("Unknown collision");
            break;
      }

    }
}
