using UnityEngine;

public class CustomAkTriggerArea : MonoBehaviour
{
    bool hasTriggered;

    [SerializeField] AK.Wwise.Event playEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            playEvent?.Post(gameObject);
            hasTriggered = true;
        }
    }
}
