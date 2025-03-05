using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GlobalEvents
{
    /// <summary>
    /// Manages global events and handles event connections and disconnections. Events are also compatible with Unity Events as long as they have the same name.
    /// </summary>
    public class GameEvents : Singleton<GameEvents>
    {
        private Dictionary<string, Delegate> eventsByName = new();
        private Dictionary<string, UnityEvent> unityEventsByName = new();

        public Dictionary<string, Delegate> EventsByName { get { return eventsByName; } }

        [SerializeField]
        [Tooltip("Assign UnityEvents here that are called when the EventHandler Action with same name is called.")]
        private List<EventAction> eventActions = new List<EventAction>();

        protected override void Awake()
        {
            base.Awake();
            RegisterUnityEvents();
        }

        private void RegisterUnityEvents()
        {
            foreach (EventAction eventAction in eventActions)
            {
                unityEventsByName.Add(eventAction.eventName, eventAction.unityEvent);
            }
        }

        #region Connect
        public static void Connect(Events e, Action listener)
        {
            Instance.AddListener(e.ToString(), listener);
        }
        public static void Connect<T>(Events e, Action<T> listener)
        {
            Instance.AddListener(e.ToString(), listener);
        }
        public static void Connect<T1, T2>(Events e, Action<T1, T2> listener)
        {
            Instance.AddListener(e.ToString(), listener);
        }
        public static void Connect(string eventName, Action listener)
        {
            Instance.AddListener(eventName, listener);
        }
        public static void Connect<T>(string eventName, Action<T> listener)
        {
            Instance.AddListener(eventName, listener);
        }
        public static void Connect<T1, T2>(string eventName, Action<T1, T2> listener)
        {
            Instance.AddListener(eventName, listener);
        }
        #endregion Connect

        #region Disconnect
        public static void Disconnect(Events e, Action listener)
        {
            Instance?.RemoveListener(e.ToString(), listener);
        }
        public static void Disconnect<T>(Events e, Action<T> listener)
        {
            Instance?.RemoveListener(e.ToString(), listener);
        }
        public static void Disconnect<T1, T2>(Events e, Action<T1, T2> listener)
        {
            Instance?.RemoveListener(e.ToString(), listener);
        }
        public static void Disconnect(string eventName, Action listener)
        {
            Instance?.RemoveListener(eventName, listener);
        }
        public static void Disconnect<T>(string eventName, Action<T> listener)
        {
            Instance?.RemoveListener(eventName, listener);
        }
        public static void Disconnect<T1, T2>(string eventName, Action<T1, T2> listener)
        {
            Instance?.RemoveListener(eventName, listener);
        }
        #endregion Disconnect

        #region Trigger
        public static void Trigger(Events e)
        {
            Instance?.InvokeEvent(e.ToString());
        }
        public static void Trigger<T>(Events e, T parameter)
        {
            Instance?.InvokeEvent(e.ToString(), parameter);
        }
        public static void Trigger<T1, T2>(Events e, T1 param1, T2 param2)
        {
            Instance?.InvokeEvent(e.ToString(), param1, param2);
        }
        public static void Trigger(string eventName)
        {
            Instance?.InvokeEvent(eventName);
        }
        public static void Trigger<T>(string eventName, T parameter)
        {
            Instance?.InvokeEvent(eventName, parameter);
        }
        public static void Trigger<T1, T2>(string eventName, T1 param1, T2 param2)
        {
            Instance?.InvokeEvent(eventName, param1, param2);
        }
        #endregion Trigger

        private void AddListener(string eventName, Delegate listener)
        {
            //Debug.Log($"Connect Listener {listener.Method} to Event {eventName}");


            if (eventsByName.TryGetValue(eventName, out Delegate thisEvent))
            {
                thisEvent = Delegate.Combine(thisEvent, listener);
                eventsByName[eventName] = thisEvent;
            }
            else
            {
                eventsByName.Add(eventName, listener);
            }
        }
        private void RemoveListener(string eventName, Delegate listener)
        {
            if (Instance == null) return;
            if (eventsByName.TryGetValue(eventName, out Delegate thisEvent))
            {
                thisEvent = Delegate.Remove(thisEvent, listener);
                eventsByName[eventName] = thisEvent;
            }
        }

        #region InvokeEvent
        private void InvokeEvent(string eventName)
        {
            if (eventsByName.TryGetValue(eventName, out Delegate thisEvent))
            {
                thisEvent?.DynamicInvoke();
                Debug.Log($"Event {eventName} has been invoked with no parameters");
            }
            TryInvokeUnityEvent(eventName);
        }

        private void InvokeEvent(string eventName, object parameter)
        {
            if (eventsByName.TryGetValue(eventName, out Delegate thisEvent))
            {
                thisEvent?.DynamicInvoke(parameter);
                Debug.Log($"Event {eventName} has been invoked with parameter {parameter}");
            }
            TryInvokeUnityEvent(eventName);
        }
        private void InvokeEvent(string eventName, object param1, object param2)
        {
            if (eventsByName.TryGetValue(eventName, out Delegate thisEvent))
            {
                thisEvent?.DynamicInvoke(param1, param2);
                Debug.Log($"Event {eventName} has been invoked with parameters {param1} and {param2}");
            }
            TryInvokeUnityEvent(eventName);
        }

        private void TryInvokeUnityEvent(string eventName)
        {
            if (unityEventsByName.TryGetValue(eventName, out UnityEvent unityEvent))
            {
                unityEvent.Invoke();
                Debug.Log($"UnityEvent {eventName} has been invoked.");
            }
        }
        #endregion InvokeEvent
    }

    [System.Serializable]
    public struct EventAction
    {
        [SerializeField] public string eventName;
        [SerializeField] public UnityEvent unityEvent;
    }

    public enum Events
    {
        None,
        GamePaused,
        MenuToggled,
        StartHoldInteract,
        StopHoldInteract,
        CollectableCollected,
        ScreenShake,
        ButtonPressed,
        ButtonPressStopped,
        CutsceneFinished,
        CutsceneStarted,
    }
}
