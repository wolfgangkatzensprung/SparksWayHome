using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    private void Start()
    {
        SceneManager.SetActiveScene(gameObject.scene);
    }
}
