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
    private PlayerController m_playerController;
    public AnySceneManager anySceneManager;

    void Awake() {
      anySceneManager = GameObject.FindGameObjectWithTag("sceneManager").GetComponentInChildren(typeof(AnySceneManager)) as AnySceneManager;
      m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
      m_playerController.jumpNum = 4;
      m_playerController.enableFire = true;
      m_playerController.sprintSpeed = 0.4f;

      m_playerController.stdTime();
    }

    void Start() {
      if(anySceneManager.portals.Contains(newScene)) {
          gameObject.SetActive(false);
      }
    }

    void OnUpdate(){

    }

    void OnTriggerEnter(Collider other){
      //Loads the appropraite scene when the player touches the portal
      if(other.gameObject.CompareTag("Player")) {
        anySceneManager.TransitionScene(newScene, currentScene);
        anySceneManager.portals.Add(newScene);
      }
    }

}
