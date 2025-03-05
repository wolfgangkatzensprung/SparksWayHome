using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class MoveToTarget : Activatable
{
    private Rigidbody rb;

    public UnityEvent onPositionReached;
    [SerializeField, Tooltip("Target Position Transform. Only works if there this doesn't use a Spline.")]
    Transform targetPos;
    [SerializeField] private float speed = 3f;
    private Vector3 targetStartPos;
    private Vector3 targetDir;

    [SerializeField, Tooltip("Spline Container. Leave empty if this should not use a Spline.")]
    private SplineContainer spline;
    private float splineLength;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Activate()
    {
        base.Activate();

        if (!gameObject.activeSelf) return;

        if (spline != null)
        {
            splineLength = spline.CalculateLength();
            StartCoroutine(MoveToTargetBySplineRoutine());
        }
        else if (targetPos != null)
        {
            targetStartPos = targetPos.position;
            targetDir = (targetPos.position - transform.position).normalized;
            StartCoroutine(MoveToTargetByTransformTargetRoutine());
        }
    }

    private IEnumerator MoveToTargetBySplineRoutine()
    {
        var distanceOnSpline = 0f;

        while (distanceOnSpline < 1)
        {
            distanceOnSpline += speed * Time.fixedDeltaTime / splineLength;
            rb.MovePosition(spline.EvaluatePosition(distanceOnSpline));
            yield return new WaitForFixedUpdate();
        }

        onPositionReached?.Invoke();
        Deactivate();
    }

    private IEnumerator MoveToTargetByTransformTargetRoutine()
    {
        var distance = Vector3.Distance(transform.position, targetStartPos);
        while (distance > 0.1f)
        {
            distance = Vector3.Distance(transform.position, targetStartPos);
            rb.MovePosition(transform.position + targetDir * speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        transform.position = targetStartPos;
        onPositionReached?.Invoke();
        Deactivate();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MoveToTarget))]
public class MoveToTargetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MoveToTarget moveToTarget = (MoveToTarget)target;

        if (GUILayout.Button("Generate TargetPos Child"))
        {
            CreateTargetPosChild(moveToTarget);
        }
    }

    private void CreateTargetPosChild(MoveToTarget moveToTarget)
    {
        GameObject targetPosChild = new GameObject("TargetPos");
        targetPosChild.transform.SetParent(moveToTarget.transform);
        targetPosChild.transform.localPosition = Vector3.zero;

        // Add the ShowGizmos script to the TargetPos child
        targetPosChild.AddComponent<ShowGizmos>();

        SerializedObject serializedObject = new SerializedObject(moveToTarget);
        SerializedProperty targetPosProperty = serializedObject.FindProperty("targetPos");
        targetPosProperty.objectReferenceValue = targetPosChild.transform;
        serializedObject.ApplyModifiedProperties();

        EditorGUIUtility.PingObject(targetPosChild);
        Debug.Log("TargetPos child created, assigned, and ShowGizmos script added.");
    }
}
#endif