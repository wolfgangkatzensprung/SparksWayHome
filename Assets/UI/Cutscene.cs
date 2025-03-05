using GlobalEvents;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.Trigger(Events.CutsceneStarted);
    }

    private void OnDisable()
    {
        GameEvents.Trigger(Events.CutsceneFinished);
    }
}
