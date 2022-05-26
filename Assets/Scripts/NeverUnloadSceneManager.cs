using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NeverUnloadSceneManager : MonoBehaviour
{
    private float m_Timer;
    private string loadScene = "";
    private AsyncOperation asyncLoad;

    public void StartMainScene() {
        Debug.Log($"Loading: Main Scene");
        asyncLoad = SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
    }

    public IEnumerator EndMainScene() {
        Debug.Log($"Loading: Main Scene");
        yield return null;
        SceneManager.UnloadSceneAsync("NeverUnload");
    }

    public IEnumerator UnloadTargetScene(string scene) {
        Debug.Log($"Unloading previous scene: {scene}");
        yield return null;
        SceneManager.UnloadSceneAsync(scene);
    }
};
