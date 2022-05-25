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

    // Start is called before the first frame update
    void Start()
    {
      m_BtnResume.onClick.AddListener(Resume);
      m_BtnMainMenu.onClick.AddListener(MainMenu);
      player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Resume()
    {
      player.GetComponent<PlayerController>().OnMenu();
    }

    void MainMenu()
    {
      SceneManager.LoadScene("MainScene");
    }
}
