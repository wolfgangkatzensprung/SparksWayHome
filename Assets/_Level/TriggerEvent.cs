using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    bool hasTriggered;
    public UnityEvent onTriggerEnter;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            onTriggerEnter?.Invoke();
            hasTriggered = true;
        }
    }
}
