using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public Button m_BtnTutorial;
    public Button m_BtnNewGame;
    public int fadeTime = 3;
    public CanvasGroup black;
    public TextMeshProUGUI LoadPercentage;
    //public NeverUnloadSceneManager MainManager;

    private bool fade = false;
    private float m_Timer;
    private string loadScene = "";
    private AsyncOperation asyncLoad;

    // Start is called before the first frame update
    void Start()
    {
      //black = GetComponent<CanvasGroup>();
      //MainManager = FindObjectOfType<NeverUnloadSceneManager>();
      m_BtnTutorial.onClick.AddListener(LaunchTutorial);
      m_BtnNewGame.onClick.AddListener(NewGame);
      Cursor.lockState = CursorLockMode.None;//locked by the playerController script
    }

    // Update is called once per frame
    void Update()
    {
      if(fade){
        m_Timer += Time.deltaTime;
        black.alpha = m_Timer/fadeTime;
        float loadProgress = asyncLoad.progress;
        LoadPercentage.text = loadProgress + "%";
      }
    }

    /*
    Sets variabls for loading the tutorial scene
    */
    void LaunchTutorial()
    {
      //MainManager.StartMainScene();
      loadScene = "TutorialScene";
      LoadScene();
    }

    void NewGame()
    {
      //MainManager.StartMainScene();
      loadScene = "HomeBaseScene";
      LoadScene();
    }

    void LoadScene()
    {
      Debug.Log($"Loading: {loadScene}");
      asyncLoad = SceneManager.LoadSceneAsync(loadScene);
      m_Timer = 0f;
      fade = true;
    }
}
