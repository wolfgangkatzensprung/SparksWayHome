using UnityEngine;

public class RollenderFelsMusic : MusicPlayer
{
    [SerializeField] AK.Wwise.Event impactSound;
    [SerializeField] float impactDelay = 2f;

    bool hasPlayedSound;

    public void PlayDelayedSound()
    {
        if (hasPlayedSound) return;

        Invoke("PlaySound", impactDelay);
        hasPlayedSound = true;
    }

    public void PlaySound()
    {
        impactSound?.Post(gameObject);
    }
}
