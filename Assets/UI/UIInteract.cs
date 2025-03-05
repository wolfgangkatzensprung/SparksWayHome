using GlobalEvents;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInteract : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI interactHint;
    [SerializeField] Slider interactHoldSlider;

    private void Awake()
    {
        interactHint.gameObject.SetActive(false);
        interactHoldSlider.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.Connect(Events.StartHoldInteract, OnStartHoldInteract);
        GameEvents.Connect(Events.StopHoldInteract, OnStopHoldInteract);
    }
    private void OnDisable()
    {
        GameEvents.Disconnect(Events.StartHoldInteract, OnStartHoldInteract);
        GameEvents.Disconnect(Events.StopHoldInteract, OnStopHoldInteract);
    }


    private void Update()
    {
        if (interactHoldSlider.gameObject.activeSelf)
        {
            interactHoldSlider.value += Time.deltaTime;
        }
    }

    private void OnStartHoldInteract()
    {
        interactHoldSlider.value = 0;
        interactHoldSlider.gameObject.SetActive(true);
    }

    private void OnStopHoldInteract()
    {
        interactHoldSlider.value = 0;
        interactHoldSlider.gameObject.SetActive(false);
    }
}
