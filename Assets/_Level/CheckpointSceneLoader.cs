using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Checkpoint))]
public class CheckpointSceneLoader : MonoBehaviour
{
    private string sceneToLoadName = "";
    private string sceneToUnloadName = "";

    private void OnEnable()
    {
        GetComponent<Checkpoint>().onActivate += LoadScene;
        GetComponent<Checkpoint>().onActivate += UnloadScene;
    }

    private void OnDisable()
    {
        GetComponent<Checkpoint>().onActivate -= LoadScene;
        GetComponent<Checkpoint>().onActivate -= UnloadScene;
    }

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneToLoadName)) return;

        SceneManager.LoadScene(sceneToLoadName, LoadSceneMode.Additive);
    }

    public void UnloadScene()
    {
        if (string.IsNullOrEmpty(sceneToUnloadName)) return;

        StartCoroutine(UnloadSceneRoutine(sceneToUnloadName));
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
}
