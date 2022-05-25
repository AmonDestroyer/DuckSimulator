using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public Transform player;
    private AsyncOperation asyncLoad;

    void OnUpdate(){
      
    }

    void OnTriggerEnter(Collider other){
      //Loads the appropraite scene when the player touches the portal
      if(other.gameObject.CompareTag("Player")){
        asyncLoad = SceneManager.LoadSceneAsync(gameObject.name);
      }
    }
}
