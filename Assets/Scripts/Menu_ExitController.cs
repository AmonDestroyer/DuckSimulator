using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu_ExitController : MonoBehaviour
{

    public Button m_BtnResume;
    public Button m_BtnOptions;
    public Button m_BtnMainMenu;

    private GameObject player;
    private AnySceneManager anySceneManager;

    // Start is called before the first frame update
    void Start()
    {
      m_BtnResume.onClick.AddListener(Resume);
      m_BtnMainMenu.onClick.AddListener(MainMenu);
      player = GameObject.Find("Player");
      anySceneManager = GameObject.FindGameObjectWithTag("sceneManager").GetComponentInChildren(typeof(AnySceneManager)) as AnySceneManager;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Resume()
    {
      player.GetComponent<PlayerController>().stdTime();
    }

    void MainMenu()
    {
      player.GetComponent<PlayerController>().stdTime();
      int currentScene = anySceneManager.m_NewScene;
      anySceneManager.TransitionScene(1, currentScene);
    }
}
