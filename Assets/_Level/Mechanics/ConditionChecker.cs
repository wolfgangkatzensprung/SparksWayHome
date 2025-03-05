using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConditionChecker : MonoBehaviour
{
    [SerializeField] List<GameObject> observedGameObjects;

    [SerializeField] ConditionType conditionType;

    [SerializeField, Tooltip("Zielrotation in Grad")] Vector3 targetRotationAngles;

    [SerializeField, Tooltip("Toleranz = Wie viel darf es vom Ziel abweichen, ohne als inkorrekt zu gelten")]
    float tolerance = 0.1f;

    [SerializeField]
    UnityEvent onConditionSuccess;

    public enum ConditionType
    {
        Rotation
    }

    private void Update()
    {
        if (conditionType == ConditionType.Rotation)
        {
            foreach (var go in observedGameObjects)
            {
                var angleDifference = Quaternion.Angle(go.transform.rotation, Quaternion.Euler(targetRotationAngles));
                if (angleDifference <= tolerance)
                {
                    onConditionSuccess?.Invoke();
                    enabled = false;
                    Debug.Log($"Target condition met: Rotation of {go.name} is correct. Unity Event triggered and ConditionChecker disabled.");
                }
            }
        }
    }
}
