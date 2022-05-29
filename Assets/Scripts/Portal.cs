using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public Transform player;
    public int newScene;
    public int currentScene;
    //public NeverUnloadSceneManager MainManager;
    public AnySceneManager anySceneManager;

    void Start() {
      anySceneManager = GameObject.FindGameObjectWithTag("sceneManager").GetComponentInChildren(typeof(AnySceneManager)) as AnySceneManager;
    }

    void OnUpdate(){
      
    }

    void OnTriggerEnter(Collider other){
      //Loads the appropraite scene when the player touches the portal
      if(other.gameObject.CompareTag("Player")) {
        anySceneManager.TransitionScene(newScene, currentScene);
      }
    }

}
