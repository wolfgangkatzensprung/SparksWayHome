using GlobalEvents;
using System;
using System.Collections;
using UnityEngine;

public class ScreenShakeTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("Custom Position INSTEAD of Transform Position")]
    bool useCustomPosition;

    [SerializeField, Tooltip("This Position is used when UseCustomPosition is enabled")]
    Vector3 customPosition;

    [SerializeField, Tooltip("Duration of the Screenshake. The last applied Duration overrides the current duration.")]
    float duration = .5f;

    [SerializeField, Tooltip("Strength of the Screenshake. Strength adds up.")]
    float strength = 2f;

    [SerializeField, Tooltip("Try find Activatable on this GameObject and connect the ScreenShake Trigger to its Activate() Method")]
    bool connectToActivatable;

    [SerializeField, Range(1, 20), Tooltip("How many Screen Shakes should be triggered in a row")]
    int shakeAmount = 1;

    [SerializeField, Tooltip("Delay between each consecutive shake trigger")]
    float delay = 1.0f;

    private void OnEnable()
    {
        if (!connectToActivatable) return;

        if (TryGetComponent(out Activatable a))
        {
            Debug.Log($"Activatable found by Screenshake. Connecting...");
            a.onActivatableEnabled += TriggerScreenShake;
        }
    }

    private void OnDisable()
    {
        if (!connectToActivatable) return;

        if (TryGetComponent(out Activatable a))
        {
            a.onActivatableEnabled -= TriggerScreenShake;
        }
    }

    public void TriggerScreenShake()
    {
        var pos = useCustomPosition ? customPosition : transform.position;
        var screenShakeEventArgs = new ScreenShakeEventArgs(pos, strength, duration);

        StartCoroutine(ScreenShakeRoutine(screenShakeEventArgs));
    }

    private IEnumerator ScreenShakeRoutine(ScreenShakeEventArgs screenShakeEventArgs)
    {
        for (int i = 0; i < shakeAmount; i++)
        {
            GameEvents.Trigger(Events.ScreenShake, screenShakeEventArgs);
            yield return new WaitForSeconds(delay);
        }
    }
}

public class ScreenShakeEventArgs : EventArgs
{
    public Vector3 position;
    public float strength;
    public float duration;

    public ScreenShakeEventArgs(Vector3 position, float strength, float duration)
    {
        this.position = position;
        this.strength = strength;
        this.duration = duration;
    }

    public override string ToString()
    {
        return $"Screenshake Pos: {position}, Strength: {strength}, Duration: {duration}";
    }
}
