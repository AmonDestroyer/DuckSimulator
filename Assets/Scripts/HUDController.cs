using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public int m_FeatherCount;
    private int m_FeatherMaxCount;
    public TextMeshProUGUI FeatherCountText;
    public GameObject player;
    private PlayerController m_PlayerController;

     //VOLUME SLIDE VARIABLE TO SET VOLUME SLIDE UPON SCENE CHANGE
    public GameObject SliderObject;
    private Slider m_VolumeSlider;


    // Start is called before the first frame update
    void Start()
    {
        m_FeatherCount = 0;
        SetCountText();

        //m_PlayerController = player.GetComponent<PlayerController>();
    }

    public void SetCountText()
    {
        FeatherCountText.text = "Feathers: " + m_FeatherCount.ToString() + " / " + m_FeatherMaxCount.ToString();
    }

    // CODE FOR DETECTIN WHEN A NEW SCENE HAS LOADED
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "MainScene"){
            return;
        }
        //VOLUME SLIDER 
        m_VolumeSlider = SliderObject.GetComponent<Slider>();

        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);

        //VOLUME SLIDER - BELOW MAINTAINS SLIDER VALUE FROM SCENE TO SCENE
        m_VolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        

        if(scene.name == "TutorialScene"){
            m_PlayerController = player.GetComponent<PlayerController>();
            m_PlayerController.health = 1.0f;
            m_FeatherMaxCount = GameObject.FindGameObjectsWithTag("Feather").Length; //TutorialMaxFeathers;
            m_FeatherCount = 0;
            SetCountText();
        } else if(scene.name == "PondScene"){
            m_PlayerController = player.GetComponent<PlayerController>();
            m_PlayerController.health = 1.0f;
            m_FeatherMaxCount = GameObject.FindGameObjectsWithTag("Feather").Length;  // PondMaxFeathers;
            m_FeatherCount = 0;
            SetCountText();
        } else if(scene.name == "ForestScene"){
            m_PlayerController = player.GetComponent<PlayerController>();
            m_PlayerController.health = 1.0f;
            m_PlayerController.jumpHeight = 15.0f;
            m_FeatherMaxCount = GameObject.FindGameObjectsWithTag("Feather").Length; // ForestMaxFeathers;
            m_FeatherCount = 0;
            SetCountText();
        } else if(scene.name == "HomeBaseScene"){
            m_PlayerController = player.GetComponent<PlayerController>();
            m_PlayerController.health = 1.0f;
            m_FeatherMaxCount = 0; // ForestMaxFeathers;
            m_FeatherCount = 0;
            SetCountText();
        }
    }



}
