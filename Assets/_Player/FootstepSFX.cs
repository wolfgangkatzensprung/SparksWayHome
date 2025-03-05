using UnityEngine;

public class FootstepSFX : MonoBehaviour
{
    [SerializeField]
    AK.Wwise.Event Footsteps;

    [SerializeField]
    AK.Wwise.Switch metalSurface;   
    
    [SerializeField]
    AK.Wwise.Switch grassSurface;

    [SerializeField]
    AK.Wwise.Switch stoneSurface;

    public void PlayFootstepSound()
    {
        Footsteps.Post(gameObject);
    }

    public void SetMetalSurface()
    {
        metalSurface.SetValue(gameObject);
    }

    public void SetGrassSurface()
    {
        grassSurface.SetValue(gameObject);
    }

    public void SetStoneSurface()
    {
        stoneSurface.SetValue(gameObject);
    }
}
