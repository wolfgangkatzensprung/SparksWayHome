using UnityEngine;

[RequireComponent (typeof(PlayerSetter))]
public class VcamFels : MonoBehaviour
{
    PlayerSetter playerSetter;

    private void Awake()
    {
        playerSetter = GetComponent<PlayerSetter>();
    }

    void OnEnable()
    {
        playerSetter.DisablePlayerInput();
        Invoke("EndCutscene", 4f);
    }

    void EndCutscene()
    {
        playerSetter.EnablePlayerInput();
        playerSetter.ForceCancelInteraction();
        gameObject.SetActive(false);
    }

}
