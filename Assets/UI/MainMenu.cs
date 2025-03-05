using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        var player = FindObjectOfType<Player>();

        if (player != null)
        {
            player.DestroyAll();
        }
    }
}
