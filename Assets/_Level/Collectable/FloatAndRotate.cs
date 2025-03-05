using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{ 
    private Transform target;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;

    private Vector3 initialPosition;

    private void Start()
    {
        target = transform.GetChild(0);
        initialPosition = target.position;
    }

    private void Update()
    {
        if (target == null) return;
        
        target.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        target.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}