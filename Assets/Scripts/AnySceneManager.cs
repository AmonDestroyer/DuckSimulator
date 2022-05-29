using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AnySceneManager : MonoBehaviour
{
    // https://www.youtube.com/watch?v=S9h_iu4Zx5I
    // concept from Egon Everest: Unity - Moving Objects Between Scenes
    public static AnySceneManager anySceneManager;
    //fade in/fade out control
    public CanvasGroup black;
    public TextMeshProUGUI LoadPercentage;
    public float fadeDuration = 2f; //Controls fade in and out duration.

    // for loading purposes
    private AsyncOperation m_AsyncLoad;
    private bool m_GameStart;
    private bool m_FadeOut;
    private bool m_FadeIn;
    private float m_Timer;
    private float m_LoadProgress;
    

    void Awake() {
        anySceneManager = this;

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive); // main menu load
        m_FadeOut = false;
        m_FadeIn = false;
        m_GameStart = true;
        m_LoadProgress = 0.0f;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update() {
        if(m_LoadProgress == 100f) { // fade out complete; fade in
            m_FadeOut = false;
            m_FadeIn = true;
      }
    }

    void FixedUpdate() {
        if(m_FadeOut) { // controls fade out of scene
            m_Timer += Time.deltaTime;
            black.alpha = m_Timer/fadeDuration;
            m_LoadProgress = m_AsyncLoad.progress;
            LoadPercentage.text = m_LoadProgress + "%";
        }
        if(m_FadeIn) {
            m_Timer += Time.deltaTime;
            black.alpha = 1 - m_Timer/fadeDuration;
            if (m_Timer > fadeDuration) // done fading in
                m_FadeIn = false;
        }
    }

    public void TransitionScene(int newScene, int currentScene) {
        LoadScene(newScene);
        UnloadScene(currentScene);
    }

    void UnloadScene(int scene) {
        StartCoroutine(Unload(scene));
    }
    
    void LoadScene(int scene) {
        m_Timer = 0f;
        m_FadeOut = true;
        Debug.Log($"Loading: {scene}");
        m_AsyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
    }

    IEnumerator Unload(int scene) {
        yield return null;
        Debug.Log($"Unloading: {scene}");
        SceneManager.UnloadSceneAsync(scene);
    }
};
