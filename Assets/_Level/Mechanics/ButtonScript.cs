using System.Collections.Generic;
using GlobalEvents;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ButtonScript : MonoBehaviour, IInteractable
{
    Player player;  // the player that is currently interacting
    ButtonParticles buttonParticles;

    [SerializeField, Tooltip("Normal = Button Press triggert Activation." +
        "\nHoldPress = Activation wird ausgefuehrt, solange der Button gepresst ist. Wenn man loslaesst, wird alles wieder deaktiviert.")]
    public ButtonMode buttonMode = ButtonMode.SingleUse;

    [SerializeField, Tooltip("Activatables that don't have Delay. These are immediately activated.")]
    internal List<Activatable> activatables = new();

    [SerializeField, Tooltip("Activatables with Delay. These are activated after the specified time in seconds.")]
    internal List<ActivatableWithDelay> activatablesWithDelays = new();

    [SerializeField, Tooltip("Global Events on Button Press")]
    internal List<string> eventNames;

    [SerializeField, Tooltip("Unity Events on Button Press"), FormerlySerializedAs("buttonEvent")]
    internal UnityEvent onButtonPressed;

    [SerializeField]
    private AK.Wwise.Event buttonPressSoundEvent;
    [SerializeField]
    private AK.Wwise.Event orbSoundEvent;


    float interactTime; // the Time.time when Player starts Interact
    float longestDelay = 0f;

    bool hasBeenUsed;

    private void Awake()
    {
        buttonParticles = GetComponentInChildren<ButtonParticles>(true);
        buttonParticles.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (buttonMode != ButtonMode.HoldPress) return;
        if (player == null) return;

        CheckButtonHold();
    }

    private void CheckButtonHold()
    {
        bool allowHoldInteraction = Vector3.Distance(transform.position, player.transform.position) <= player.Interaction.interactionDistance && player.Input.IsInteractPressed;

        if (!allowHoldInteraction)
        {
            player = null;
            StopInteract();
        }
    }

    public bool Interact(Player player)
    {
        if (!enabled) return false;
        if (buttonMode == ButtonMode.SingleUse && hasBeenUsed) return false;
        if (Time.time <= interactTime + longestDelay) return false;

        buttonParticles.gameObject.SetActive(true);
        buttonPressSoundEvent.Post(gameObject);
        orbSoundEvent.Post(gameObject);

        interactTime = Time.time;

        this.player = player;

        foreach (var activatable in activatables)
        {
            activatable.Activate();
        }

        if (buttonMode == ButtonMode.SingleUse)
        {
            PerformNormalButtonInteractions();
        }

        hasBeenUsed = true;

        GameEvents.Trigger(Events.ButtonPressed);

        return true;
    }

    private void PerformNormalButtonInteractions()
    {
        foreach (var activatableWithDelay in activatablesWithDelays)
        {
            longestDelay = Mathf.Max(longestDelay, activatableWithDelay.Delay);
            activatableWithDelay.Activatable.Invoke(nameof(Activatable.Activate), activatableWithDelay.Delay);
            Debug.Log($"Activatable started. Longest Button Delay is {longestDelay}s. Button is disabled for this time.");
        }

        foreach (var eventName in eventNames)
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                GameEvents.Trigger(eventName);
                Debug.Log("Event triggered by ButtonPress: " + eventName);
            }
        }

        onButtonPressed?.Invoke();
    }

    public bool StopInteract()
    {
        buttonParticles.gameObject.SetActive(false);
        foreach (var wwiseEvent in GetComponents<AkEvent>())
        {
            wwiseEvent.Stop(1);
        }

        foreach (var activatable in activatables)
        {
            activatable.Deactivate();
        }

        GameEvents.Trigger(Events.ButtonPressStopped);
        return true;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}

public enum ButtonMode
{
    SingleUse = 0,
    MultiUse = 10,
    HoldPress = 20
}

[System.Serializable]
public class ActivatableWithDelay
{
    public Activatable Activatable;
    public float Delay;
}


#if UNITY_EDITOR
[CustomEditor(typeof(ButtonScript))]
public class ButtonScriptEditor : Editor
{
    SerializedProperty buttonModeProp;

    private void OnEnable()
    {
        buttonModeProp = serializedObject.FindProperty("buttonMode");
    }

    public override void OnInspectorGUI()
    {
        ButtonScript buttonScript = (ButtonScript)target;

        EditorGUILayout.PropertyField(buttonModeProp, new GUIContent("Button Mode"));

        if (buttonScript.buttonMode == ButtonMode.HoldPress)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("activatables"), true);
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("activatables"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("activatablesWithDelays"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventNames"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onButtonPressed"), true);
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonPressSoundEvent"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("orbSoundEvent"), true);

        //if (GUILayout.Button("Clear all"))
        //{
        //    buttonScript.activatables.Clear();
        //    buttonScript.activatablesWithDelays.Clear();
        //    buttonScript.eventNames.Clear();
        //    buttonScript.onButtonPressed.RemoveAllListeners();
        //    EditorUtility.SetDirty(buttonScript);
        //}

        serializedObject.ApplyModifiedProperties();
    }
}
#endif