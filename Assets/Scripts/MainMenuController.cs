using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Button m_BtnTutorial;
    public int fadeTime = 3;
    public CanvasGroup black;

    private bool fade = false;
    private float m_Timer;
    private string loadScene = "";

    // Start is called before the first frame update
    void Start()
    {
      //black = GetComponent<CanvasGroup>();
      m_BtnTutorial.onClick.AddListener(LaunchTutorial);
      Cursor.lockState = CursorLockMode.None;//locked by the playerController script
    }

    // Update is called once per frame
    void Update()
    {
      if(fade){
        m_Timer += Time.deltaTime;
        black.alpha = m_Timer/fadeTime;
        if (m_Timer > fadeTime)
          SceneManager.LoadScene(loadScene);
      }
    }

    /*
    Sets variabls for loading the tutorial scene
    */
    void LaunchTutorial()
    {
      Debug.Log("Running Tutorial");
      m_Timer = 0f;
      fade = true;
      loadScene = "TutorialScene";
    }
}
