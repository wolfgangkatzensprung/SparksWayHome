using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Additively Load Scene
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded) return;

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Asynchronously Unload Scene
    /// </summary>
    /// <param name="sceneName"></param>
    public void UnloadScene(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded) return;

        StartCoroutine(UnloadSceneRoutine(sceneName));
    }

    IEnumerator UnloadSceneRoutine(string sceneName)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        if (asyncUnload.isDone)
        {
            Debug.Log(sceneName + " was unloaded asyncly!");
        }
    }

    public void QuitToMenu()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "Main" && scene.name != "UI")
            {
                UnloadScene(scene.name);
            }
        }

        LoadScene("MainMenu");
    }

    public void LoadCredits()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "Main")
            {
                UnloadScene(scene.name);
            }
        }

        LoadScene("Credits");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
