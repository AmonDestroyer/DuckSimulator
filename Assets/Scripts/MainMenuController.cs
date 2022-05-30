using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public Button btnTutorial;
    public Button btnNewGame;
    public Button btnOptions;

    //Private Variables Start
    private AnySceneManager m_AnySceneManager;

    // Start is called before the first frame update
    void Start()
    {
      btnTutorial.onClick.AddListener(LaunchTutorial);
      btnNewGame.onClick.AddListener(NewGame);
      btnOptions.onClick.AddListener(Options);
      Cursor.lockState = CursorLockMode.None;//locked by the playerController script

      m_AnySceneManager = GameObject.FindGameObjectWithTag("sceneManager").GetComponentInChildren(typeof(AnySceneManager)) as AnySceneManager;

      if(m_AnySceneManager == null) { // try again, but with inactive objects too
        m_AnySceneManager = GameObject.FindGameObjectWithTag("sceneManager").GetComponentInChildren(typeof(AnySceneManager), true) as AnySceneManager;
      }

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    Sets variabls for loading the tutorial scene
    */
    void LaunchTutorial()
    {
      m_AnySceneManager.TransitionScene(2, 1);
    }

    void NewGame()
    {
      m_AnySceneManager.TransitionScene(3, 1);
    }

    void Options()
    {
      //Arbirary class used for when the options menu is selected
    }

}
