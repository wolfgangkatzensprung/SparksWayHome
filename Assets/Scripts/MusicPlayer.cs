using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event musicEvent;

    public void PlayMusic()
    {
        musicEvent?.Post(gameObject);
    }
    
    public void StopMusic()
    {
        musicEvent?.Stop(gameObject, 1);
    }
}
