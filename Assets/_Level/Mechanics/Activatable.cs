using System;
using UnityEngine;

public class Activatable : MonoBehaviour
{
    public Action onActivatableEnabled;
    public Action onActivatableDisabled;

    [SerializeField]
    private AK.Wwise.Event playEvent;

    protected virtual void OnEnable()
    {
        playEvent?.Post(gameObject);
        onActivatableEnabled?.Invoke();
    }

    protected virtual void OnDisable()
    {
        playEvent?.Stop(gameObject, 1);
        onActivatableDisabled?.Invoke();
    }

    public virtual void Activate()
    {
        if (!gameObject.activeSelf) return;

        enabled = true;
    }

    public virtual void Deactivate()
    {
        if (!gameObject.activeSelf) return;

        enabled = false;
    }
}
