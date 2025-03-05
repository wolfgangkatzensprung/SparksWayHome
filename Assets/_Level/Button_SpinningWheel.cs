using GlobalEvents;
using System.Collections;
using UnityEngine;

public class Button_SpinningWheel : MonoBehaviour
{
    Coroutine spinningRoutine;
    [SerializeField] Vector3 rotation;

    void OnEnable()
    {
        GameEvents.Connect(Events.ButtonPressed, OnButtonPressed);
        GameEvents.Connect(Events.ButtonPressStopped, OnButtonPressStopped);
    }

    void OnDisable()
    {
        GameEvents.Disconnect(Events.ButtonPressed, OnButtonPressed);
        GameEvents.Disconnect(Events.ButtonPressStopped, OnButtonPressStopped);
    }

    void OnButtonPressed()
    {
        spinningRoutine = StartCoroutine(SpinningRoutine());
    }

    void OnButtonPressStopped()
    {
        StopCoroutine(spinningRoutine);
    }

    IEnumerator SpinningRoutine()
    {
        while (true)
        {
            transform.Rotate(rotation * Time.deltaTime);
            yield return null;
        }
    }
}
