using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GearWheel : Activatable
{
    Rigidbody rb;
    MeshRenderer mr;
    Player player;

    enum AxisType
    {
        Horizontal,
        Vertical,
        IgnorePlayer
    }

    [SerializeField, Tooltip("Currently not doing anything but maybe in the future!")]
    AxisType axisType = AxisType.Horizontal;

    [SerializeField]
    Vector3 rotationSpeed = new Vector3(0f, 0f, 10f);

    [SerializeField, Range(0f, 360f), Tooltip("Maximum Rotation that can be applied to this object. Leave at 0 for infinite rotation.")]
    float maxRotationAngles = 0f;

    private float totalRotationAmount = 0f;

    [SerializeField, Tooltip("Rotation Interval (in euler angles) to be applied per Activation. Leave at 0 for consecutive rotation.")]
    float rotationAnglesInterval = 0f;
    float currentIntervalRotation;
    Quaternion onEnableRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        onEnableRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        Vector3 rotationAmount = rotationSpeed * Time.deltaTime;

        if (maxRotationAngles > 0f)
        {
            float remainingRotation = maxRotationAngles - totalRotationAmount;
            if (remainingRotation <= 0f)
            {
                Deactivate();
                return;
            }

            if (rotationAmount.magnitude > remainingRotation)
            {
                rotationAmount = rotationAmount.normalized * remainingRotation;
            }
        }

        if (rotationAnglesInterval > 0f)
        {
            float remainingIntervalRotation = rotationAnglesInterval - currentIntervalRotation;
            if (remainingIntervalRotation <= 0f) return;

            if (rotationAmount.magnitude > remainingIntervalRotation)
            {
                rotationAmount = rotationAmount.normalized * remainingIntervalRotation;
            }
        }

        Quaternion deltaRotation = Quaternion.Euler(rotationAmount);
        rb.MoveRotation(rb.rotation * deltaRotation);

        totalRotationAmount += rotationAmount.magnitude;
        currentIntervalRotation += rotationAmount.magnitude;

        if (currentIntervalRotation >= rotationAnglesInterval && rotationAnglesInterval > 0f)
        {
            currentIntervalRotation = 0f;
            enabled = false;
        }


        if (player != null)
        {
            Vector3 relativePosition = player.Rigidbody.position - rb.position;
            Quaternion rotationDelta = Quaternion.AngleAxis(
                rb.angularVelocity.magnitude * Mathf.Rad2Deg * Time.fixedDeltaTime,
                rb.angularVelocity.normalized
            );
            Quaternion newRotation = rotationDelta * player.Rigidbody.rotation;

            //Debug.Log($"Player Position: {player.Rigidbody.position}, Rotating Object Position: {rb.position}, Relative Position: {relativePosition}, Rotation Delta: {rotationDelta}, New Rotation: {newRotation}");

            player.Rigidbody.MoveRotation(newRotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled) return;
        if (axisType != AxisType.Horizontal) return;

        if (collision.collider.CompareTag("Player"))
        {
            player = collision.collider.GetComponent<Player>();
            //player.Movement.ExternalRotationTarget = transform;
            //player.Movement.ExternalRotationSpeed = rotationSpeed.magnitude / 180f * -Mathf.PI * Mathf.Sign(rotationSpeed.z);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!enabled) return;
        if (axisType != AxisType.Horizontal) return;

        if (collision.collider.CompareTag("Player"))
        {
            //player.Movement.ExternalRotationTarget = null;
            //player.Movement.ExternalRotationSpeed = 0f;
            //player.Movement.ExternalVelocity = Vector3.zero;
            player = null;
        }
    }

    public void SetRotationSpeedX(float turnSpeed)
    {
        rotationSpeed.x = turnSpeed;
    }

    public void SetRotationSpeedY(float turnSpeed)
    {
        rotationSpeed.y = turnSpeed;
    }

    public void SetRotationSpeedZ(float turnSpeed)
    {
        rotationSpeed.z = turnSpeed;
    }

    public void ResetTotalRotation()
    {
        totalRotationAmount = 0;
    }
}
