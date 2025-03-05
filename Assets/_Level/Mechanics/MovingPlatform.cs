using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : Activatable
{
    Rigidbody rb;
    Player player;
    [SerializeField, Tooltip("Der ganz normale Collider, fuer Gizmos")]
    BoxCollider boxCollider;

    [SerializeField] Vector3 offset;
    [SerializeField] float moveTime = 2.0f;
    [SerializeField] AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private Vector3 startPosition;  // start position for gizmos
    private Vector3 moveStartPosition;  // position from where a new move starts
    private Vector3 targetPosition; // position where the move ends and swaps direction
    private Vector3 nextPosition;   // position to reach this frame
    private Vector3 previousPosition;   // position of last frame
    private float targetOffsetMultiplier;
    
    float moveTimeCounter = 0f;
    float startMoveTime;

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
        StartCoroutine(LateFixedUpdate());
    }

    void Start()
    {
        startPosition = transform.position;
        previousPosition = startPosition;
        moveStartPosition = startPosition;
        offset = transform.TransformDirection(offset);
        targetPosition = moveStartPosition + offset;
        startMoveTime = moveTime;
    }

    IEnumerator LateFixedUpdate()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        yield return new WaitForEndOfFrame();
        yield return wait;

        while (enabled)
        {
            yield return wait;
            MovePlatform();
        }
    }

    private void MovePlatform()
    {
        moveTimeCounter += Time.fixedDeltaTime;

        float journeyProgress = moveTimeCounter / moveTime;
        float curveTime = moveCurve.Evaluate(journeyProgress);
        nextPosition = Vector3.Lerp(moveStartPosition, targetPosition, curveTime);
        rb.MovePosition(nextPosition);

        if (journeyProgress >= 1.0f)
        {
            (moveStartPosition, targetPosition) = (targetPosition, moveStartPosition);
            moveTimeCounter = 0f;

            if ((targetOffsetMultiplier != 0) && (Vector3.Distance(startPosition, moveStartPosition) < Vector3.Distance(startPosition, targetPosition)))
            {
                targetPosition = startPosition + targetOffsetMultiplier * offset;
                moveTime = startMoveTime * targetOffsetMultiplier;
                targetOffsetMultiplier = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player") && player == null)
        {
            player = col.GetComponent<Player>();
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player") && player != null)
        {
            player = null;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        var size = boxCollider.size;
        var colCenter = boxCollider.center;

        var matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.matrix = matrix;

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.InverseTransformPoint(targetPosition + colCenter), size);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.InverseTransformPoint(moveStartPosition + colCenter), size);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(offset * (1 / transform.localScale.y) + colCenter, size);
        }
    }

    public void MultiplyOffset(float multiplier)
    {
        targetOffsetMultiplier = multiplier;
    }

    public void ResetYToCurrent()
    {
        targetPosition.y = transform.position.y;
        moveStartPosition.y = transform.position.y;
        startPosition.y = transform.position.y;
    }
}