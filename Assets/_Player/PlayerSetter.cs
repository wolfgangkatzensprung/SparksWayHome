using UnityEngine;

public class PlayerSetter : MonoBehaviour
{
    public void DisablePlayerInput()
    {
        FindObjectOfType<Player>().Input.enabled = false;
    }

    public void EnablePlayerInput()
    {
        FindObjectOfType<Player>().Input.enabled = true;
    }

    public void ForceCancelInteraction()
    {
        FindObjectOfType<Player>().Input.InteractCanceled.Invoke();
    }
}
