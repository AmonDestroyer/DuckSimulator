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
    public GameObject player;
    public GameObject ui_interface;
    public GameObject camera;

    public float fadeDuration = 2f; //Controls fade in and out duration.

    // for loading purposes
    private AsyncOperation m_AsyncLoad;
    private bool m_FadeOut;
    private bool m_LoadScene;
    private bool m_FadeIn;
    private bool m_ActivatePlayer;
    private float m_Timer;
    private float m_LoadProgress;
    public int m_NewScene;
    public int m_CurrentScene;

    public bool debug = false;


    void Awake() {
        anySceneManager = this;
        player.SetActive(false);
        ui_interface.SetActive(false);
        camera.SetActive(false);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive); // main menu load
        m_FadeOut = false;
        m_LoadScene = false;
        m_FadeIn = false;
        m_ActivatePlayer = false;
        m_LoadProgress = 0.0f;
        Cursor.lockState = CursorLockMode.None;
    }

    void FixedUpdate() {
        if(m_FadeOut) { // controls fade out of scene
            m_Timer += Time.deltaTime;
            black.alpha = m_Timer/fadeDuration;

            if(black.alpha == 1) { // fade-out complete; start fade-in
                if(debug)
                    Debug.Log($"Fade-out complete! {black.alpha}");
                if(m_ActivatePlayer) {
                    player.SetActive(true); // otherwise, enable
                    ui_interface.SetActive(true);
                    camera.SetActive(true);
                } else {
                    player.SetActive(false); // disable player at menu
                    ui_interface.SetActive(false);
                    camera.SetActive(false);
                }
                LoadScene(m_NewScene); // fade completed; load new scene
                UnloadScene(m_CurrentScene);
                m_Timer = 0.0f;
                m_FadeOut = false;
                m_LoadScene = true;
            }
        }
        if(m_LoadScene) {
            if (debug)
                Debug.Log($"{m_LoadProgress}");
            m_LoadProgress = m_AsyncLoad.progress;
            LoadPercentage.text = m_LoadProgress*100f + "%";
            if(m_LoadProgress == 1f) { // scene loaded; fade in
                m_LoadScene = false;
                m_FadeIn = true;
            }
        }
        if(m_FadeIn) {
            m_Timer += Time.deltaTime;
            black.alpha = 1 - (m_Timer/fadeDuration);
            if(debug)
                Debug.Log($"Fade-in: {black.alpha}");
            if (black.alpha == 0) { // done fading in
                if(debug)
                    Debug.Log("Fade-in complete!");
                m_FadeIn = false;
            }
        }

    }

    public void TransitionScene(int newScene, int currentScene) {
        if(newScene != 1) {
            m_ActivatePlayer = true;
        } else {
            m_ActivatePlayer = false;
        }
        m_Timer = 0f;
        m_NewScene = newScene;
        m_CurrentScene = currentScene;
        m_FadeOut = true;
    }

    void UnloadScene(int scene) {
        StartCoroutine(Unload(scene));
    }

    void LoadScene(int scene) {
        if(debug)
            Debug.Log($"Loading: {scene}");
        m_AsyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
    }

    IEnumerator Unload(int scene) {
        yield return null;
        if(debug)
            Debug.Log($"Unloading: {scene}");
        SceneManager.UnloadSceneAsync(scene);
    }
};
