using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public Button btnTutorial;
    public Button btnNewGame;
    //public int fadeTime = 3;
    //public CanvasGroup black;
    //public TextMeshProUGUI LoadPercentage;
    private AnySceneManager m_AnySceneManager;
    /*
    private bool fade = false;
    private float m_Timer;
    private string loadScene = "";
    private AsyncOperation asyncLoad;
    */
    // Start is called before the first frame update
    void Start()
    {
      //black = GetComponent<CanvasGroup>();
      //MainManager = FindObjectOfType<NeverUnloadSceneManager>();
      btnTutorial.onClick.AddListener(LaunchTutorial);
      btnNewGame.onClick.AddListener(NewGame);
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
      //MainManager.StartMainScene();
      //loadScene = "TutorialScene";
      m_AnySceneManager.TransitionScene(2, 1);
    }

    void NewGame()
    {
      //MainManager.StartMainScene();
      //loadScene = "HomeBaseScene";
      m_AnySceneManager.TransitionScene(3, 1);
    }

    /*void LoadScene()
    {
      Debug.Log($"Loading: {loadScene}");
      asyncLoad = SceneManager.LoadSceneAsync(loadScene);
      m_Timer = 0f;
      fade = true;
    }*/
}
