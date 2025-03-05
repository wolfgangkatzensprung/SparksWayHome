using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pendulum : Activatable
{
    Rigidbody rb;
    Player player;

    [SerializeField] Vector3 offset;
    [SerializeField] float moveTime = 2.0f;
    [SerializeField] AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private Vector3 startRotation;  // rotation from where a new move starts
    private Vector3 moveStartRotation;  // rotation from where a new move starts
    private Vector3 targetRotation;
    private float offsetQueue;
    private float multiplier = 1;

    Vector3 previousVelocity = Vector3.zero;

    float moveTimeCounter = 0f;
    float startMoveTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        startRotation = transform.localRotation.eulerAngles;
        moveStartRotation = startRotation;
        targetRotation = moveStartRotation + offset;
        startMoveTime = moveTime;
    }

    private void Update()
    {
        moveTimeCounter += Time.deltaTime;
    }

    void FixedUpdate()
    {
        float journeyProgress = moveTimeCounter / moveTime;
        float curveTime = moveCurve.Evaluate(journeyProgress);
        rb.MoveRotation(Quaternion.Euler(Vector3.Lerp(moveStartRotation, targetRotation, curveTime)));

        if (journeyProgress >= 1.0f)
        {
            (moveStartRotation, targetRotation) = (targetRotation, moveStartRotation);
            moveTimeCounter = 0f;

            if ((0 != offsetQueue) && (startRotation == moveStartRotation))
            {
                targetRotation = startRotation + (offset * offsetQueue);
                moveTime = startMoveTime * offsetQueue;
                offsetQueue = 0;
            }
        }
    }

    public void MultiplyOffset(float multiplier)
    {
        offsetQueue = multiplier;
    }
}