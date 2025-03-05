using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class SceneLoaderArea : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoadName = "";
    [SerializeField]
    private string sceneToUnloadName = "";

    bool hasLoaded;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (hasLoaded) return;
        hasLoaded = true;

        TryLoadScene();
        TryUnloadScene();
    }

    public void TryLoadScene()
    {
        if (string.IsNullOrEmpty(sceneToLoadName)) return;
        if (SceneManager.GetSceneByName(sceneToLoadName).isLoaded) return;

        Debug.Log($"Try Load Scene: {sceneToLoadName}");

        SceneManager.LoadScene(sceneToLoadName, LoadSceneMode.Additive);
    }

    public void TryUnloadScene()
    {
        if (string.IsNullOrEmpty(sceneToUnloadName)) return;
        if (!SceneManager.GetSceneByName(sceneToUnloadName).isLoaded) return;

        Debug.Log($"Try Unload Scene: {sceneToUnloadName}");

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
