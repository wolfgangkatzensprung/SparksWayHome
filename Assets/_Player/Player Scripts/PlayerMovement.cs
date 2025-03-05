using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Player player;
    private Transform playerModel;
    private Rigidbody rb;
    Animator anim;
    private float _animationBlend;
    private CapsuleCollider footCollider;

    [SerializeField] PlayerInput input;

    [SerializeField, Range(0.1f, 35f), Tooltip("Player Movement Speed")]
    private float speed = 10f;

    [SerializeField, Tooltip("Maximum Look up and Look down")]
    private Vector2 yRotationBounds = new Vector2(-150f, 150f);

    [SerializeField, Range(10f, 480f), Tooltip("Player Model Rotation Speed")]
    private float rotationSpeed = 360f;

    [FormerlySerializedAs("rotationSmoothTime")]
    [SerializeField, Range(0.01f, 0.5f), Tooltip("Mouse Smoothing")]
    private float mouseSmoothingTime = 0.12f;

    [SerializeField, Range(0.01f, 1f), Tooltip("Mouse Sensitivity")]
    private float mouseSpeed = 1f;

    [SerializeField, Range(0.1f, 15f), Tooltip("Impulse Force applied at Start of Jump")]
    private float jumpVelocity = 15f;

    [SerializeField, Range(0.1f, 35f), Tooltip("Additive Force applied while jumping up")]
    private float jumpForce = 30f;

    [SerializeField, Range(0.1f, 55f), Tooltip("Additive Force applied while flutterjumping up")]
    private float flutterJumpForce = 30f;

    [SerializeField, Range(0.1f, 30f), Tooltip("Upward Velocity applied in the final phase of the FlutterJump")]
    private float flutterJumpFinishVelocity = 30f;

    [SerializeField, Range(0.1f, 1f), Tooltip("Maximum time Player is allowed to hold the Jump Button")]
    private float maxJumpTime = .5f;
    private float jumpTimeTimer = 0f;

    [SerializeField, Range(0.1f, 3f), Tooltip("Maximum time Player is allowed to hold the Jump Button for Flutter Jump")]
    private float maxFlutterJumpTime = .5f;
    private float flutterJumpTimeCounter = 0f;

    [SerializeField]
    private GameObject flutterSparkVfxPrefab;

    [SerializeField]
    AK.Wwise.Event sparkSoundEvent;

    [SerializeField, Range(0.01f, 0.5f), Tooltip("Coyote Time")]
    private float coyoteTime = 0.1f;
    private float coyoteTimeTimer = 0f;

    [SerializeField, Range(0.1f, 5f), Tooltip("Gravity is increased during this time after jump started")]
    float jumpGravityTime = 3f;

    private float jumpGravityTimer = 0f;

    [SerializeField, Range(1f, 30f), Tooltip("Custom Gravity Force for downward acceleration")]
    private float jumpGravityForce = 15f;

    [SerializeField, Tooltip("Player takes fall damage when falling for this height")]
    private float maxFallHeight = 10f;
    private float startFallY = 0f;

    [SerializeField, Tooltip("Ground Layers")]
    private LayerMask groundDetectionLayers;

    [SerializeField, Tooltip("Range to detect Ground Layers")]
    private float groundDetectionRadius = 1f;

    [SerializeField, Range(1f, 180f), Tooltip("Maximum Angle for Player to climb a surface")]
    private float maxClimbAngle = 60f;

    [SerializeField, Tooltip("Falling Speed is limited to this y velocity value")]
    private float maxFallSpeed = 100;

    private float mouseX;
    private float mouseY;
    private float smoothX;
    private float smoothY;
    private float smoothXVelocity;
    private float smoothYVelocity;

    bool _isGrounded;
    public bool IsGrounded
    {
        get => _isGrounded;
        private set
        {
            if (_isGrounded == value) return;
            _isGrounded = value;

            DoFallingCheck();
        }
    }

    public bool IsJumping => IsJumpingPrimary || isFlutterJumping;
    public bool IsJumpingPrimary { get; private set; }
    public bool isFlutterJumping { get; private set; }
    bool hasFlutterJumped;

    float startIdleTime;    // time when idling starts
    float maxIdleTime = 15f;    // play fidget animation after this amount of idle time

    public Action OnStartMoving;
    public Action OnStopMoving;

    bool _isMoving;
    public bool IsMoving
    {
        get => _isMoving;
        private set
        {
            if (_isMoving == value) return;
            _isMoving = value;

            if (!_isMoving)
            {
                OnStopMoving?.Invoke();
            }
            else
            {
                OnStartMoving?.Invoke();
            }
        }
    }

    float slideTimer; // tiny slideTimer for preventing bool jitter
    bool _isSliding;
    public bool IsSliding
    {
        get => _isSliding;
        private set
        {
            if (_isSliding == value) return;
            _isSliding = value;

            if (_isSliding)
            {
                slideTimer = .2f;
                footCollider.material = slideMaterial;
                anim.SetBool("Slide", true);
            }
            else
            {
                footCollider.material = footMaterial;
                anim.SetBool("Slide", false);
            }
        }
    }

    private FootstepSFX footStepSfx;
    enum SurfaceType
    {
        Stone = 0,
        Grass = 5,
        Metal = 10
    }
    private SurfaceType _currentSurface;
    private SurfaceType CurrentSurface
        {
        get => _currentSurface;
        set
        {
            if (_currentSurface == value) return;
            _currentSurface = value;

            switch (_currentSurface)
            {
                case SurfaceType.Grass:
                    footStepSfx.SetGrassSurface();
                    break;
                case SurfaceType.Metal:
                    footStepSfx.SetMetalSurface();
                    break;
                case SurfaceType.Stone: footStepSfx.SetStoneSurface(); 
                    break;
            }
        }
    }

    private float wallAvoidance;

    [SerializeField] PhysicMaterial footMaterial;
    [SerializeField] PhysicMaterial slideMaterial;

    /// <summary>
    /// Target Velocity determined by Player Input
    /// </summary>
    private Vector3 targetVelocity = Vector3.zero;

    /// <summary>
    /// Final Target Velocity determined by Player Input + External Velocity
    /// </summary>
    private Vector3 finalVelocity = Vector3.zero;

    public Vector3 ExternalVelocity;
    public float ExternalRotationSpeed = 0f;
    public Transform ExternalRotationTarget;

    [SerializeField]
    float animationBlendSpeed = 5f;

    public bool canMove;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerModel = player.ModelTrans;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        footCollider = GetComponents<CapsuleCollider>()[1];

        wallAvoidance = GetComponents<CapsuleCollider>()[0].radius + .05f;

        footStepSfx = GetComponentInChildren<FootstepSFX>();

        AssignAnimationIDs();
    }

    private void OnEnable()
    {
        player.Input.JumpStarted += TryStartJump;
        player.Input.JumpCanceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        player.Input.JumpStarted -= TryStartJump;
        player.Input.JumpCanceled -= OnJumpCanceled;
    }

    private void Start()
    {
        canMove = true;
    }

    private void Update()
    {
        if (Game.IsPaused) return;
        if (!canMove) return;

        float minSlideAngle = 5f;

        bool anySlideHit = false;
        IsGrounded = GroundedCheck(ref anySlideHit);
        IsSliding = SlidingCheck(minSlideAngle, anySlideHit);

        if (IsGrounded)
        {
            CurrentSurface = SurfaceCheck();

            startFallY = rb.position.y;
            coyoteTimeTimer = coyoteTime;
            jumpGravityTimer = 0f;
            hasFlutterJumped = false;
            anim.SetBool(_animIDGrounded, true);
            anim.SetBool(_animIDFreeFall, false);
            anim.SetBool(_animIDJump, false);
        }
        else
        {
            anim.SetBool(_animIDGrounded, false);
            coyoteTimeTimer -= Time.deltaTime;

            if (!IsJumping)
            {
                anim.SetBool(_animIDFreeFall, true);
            }
        }

        UpdateMouseInput();
        SmoothRotation();

        if (IsMoving)
            RotatePlayerModel();

        if (flutterJumpTimeCounter < maxFlutterJumpTime)
        {
            flutterJumpTimeCounter += Time.deltaTime;
        }
        else if (isFlutterJumping)
        {
            EndFlutterJump();
        }

        if (jumpTimeTimer > 0f)
        {
            jumpTimeTimer -= Time.deltaTime;
        }
        else if (IsJumpingPrimary)
        {
            EndJump();
            if (!isFlutterJumping && input.IsJumpPressed)
            {
                StartFlutterJump();
            }
        }

        if (IsMoving)
        {
            startIdleTime = Time.time;
        }
        else if (Time.time > startIdleTime + maxIdleTime)
        {
            anim.SetTrigger("Fidget");
        }

        if (slideTimer > 0)
        {
            slideTimer -= Time.deltaTime;
        }

        Shader.SetGlobalVector("_Player", transform.position + Vector3.up * footCollider.radius);
    }

    private void FixedUpdate()
    {
        if (Game.IsPaused) return;
        if (!canMove) return;
        if (player.Interaction.IsInteracting) return;

        Move(input.MoveInputVector3);

        if (IsJumpingPrimary)
        {
            if (input.IsJumpPressed)
                Jump();
        }
        else if (isFlutterJumping)
        {
            if (input.IsJumpPressed)
                FlutterJump();
        }

        if (!IsGrounded)
        {
            AddJumpGravity();
        }

        ApplyMovementVelocity();

        if (IsJumping && jumpGravityTimer > 0)
        {
            jumpGravityTimer -= Time.fixedDeltaTime;
            AddJumpGravity();
        }
    }

    #region Mouse and Rotation

    private void UpdateMouseInput()
    {
        mouseX += input.MouseInput.x;
        mouseY -= input.MouseInput.y;
        mouseY = Mathf.Clamp(mouseY, yRotationBounds.x, yRotationBounds.y);
    }

    private void SmoothRotation()
    {
        smoothX = Mathf.SmoothDamp(smoothX, mouseX * mouseSpeed, ref smoothXVelocity, mouseSmoothingTime);
        smoothY = Mathf.SmoothDamp(smoothY, mouseY * mouseSpeed, ref smoothYVelocity, mouseSmoothingTime);

        Quaternion targetRotation = Quaternion.Euler(smoothY, smoothX, 0f);
        player.CameraPivot.localRotation = targetRotation;
    }

    float smoothFactor = 12f;
    private Vector3 slopeDirection = Vector3.zero;

    private void RotatePlayerModel()
    {
        if (slideTimer > 0)
        {
            RaycastHit hit;

            if (Physics.Raycast(playerModel.position + playerModel.up * .7f, Vector3.down, out hit, 2f))
            {
                Vector3 slopeNormal = hit.normal;
                Vector3 newSlopeDirection = Vector3.ProjectOnPlane(Vector3.down, slopeNormal).normalized;
                slopeDirection = Vector3.Slerp(slopeDirection, newSlopeDirection, Time.deltaTime * smoothFactor);
            }

            Quaternion targetRotation = Quaternion.LookRotation(slopeDirection);
            playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            //Debug.Log($"Sliding at {Time.time}");
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetVelocity);
            playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            //Debug.Log($"Not sliding at {Time.time}");
            slopeDirection = Vector3.zero;  // Reset slope direction when not sliding
        }
    }

    #endregion

    private void DoFallingCheck()
    {
        var fallDistance = startFallY - rb.position.y;
        //Debug.Log($"Player fell {fallDistance} meters from Y={startFallY} to Y={rb.position.y}");
        if (fallDistance >= maxFallHeight)
        {
            player.Die();
        }
    }

    #region Jump

    private void TryStartJump()
    {
        if (Game.IsPaused) return;

        if (!IsJumpingPrimary)
        {
            if (IsGrounded || coyoteTimeTimer > 0)
            {
                StartPrimaryJump();
                return;
            }
        }

        //Debug.Log($"Try Secondary Jump. IsJumpingSecondary {isFlutterJumping}");
        if (!isFlutterJumping && !hasFlutterJumped)
        {
            StartFlutterJump();
        }
    }

    private void StartPrimaryJump()
    {
        if (player.Interaction.IsInteracting) return;

        IsJumpingPrimary = true;
        isFlutterJumping = false;
        jumpTimeTimer = maxJumpTime;
        rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);

        anim.SetBool(_animIDJump, true);
    }

    private void Jump()
    {
        if (player.Interaction.IsInteracting) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
        anim.SetBool(_animIDJump, true);
    }

    private void StartFlutterJump()
    {
        if (player.Interaction.IsInteracting) return;

        isFlutterJumping = true;
        IsJumpingPrimary = false;
        flutterJumpTimeCounter = 0f;

        if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 2, rb.velocity.z);
        }

        hasFlutterJumped = true;

        anim.SetBool(_animIDJump, true);
    }

    private void FlutterJump()
    {
        if (player.Interaction.IsInteracting) return;
        if (IsGrounded) EndFlutterJump();

        float flutterJumpTime01 = flutterJumpTimeCounter / maxFlutterJumpTime;
        anim.SetFloat(_animIDFlutterJumpTime, flutterJumpTime01);

        float maxVelocity = 7f;

        if (rb.velocity.y > maxVelocity)
        {
            AddJumpGravity();
        }
        else if (rb.velocity.y < 0)
        {
            var flutterFactor = Mathf.Lerp(.5f, Mathf.Abs(rb.velocity.y), flutterJumpTimeCounter);
            rb.AddForce(Vector3.up * flutterJumpForce * flutterFactor, ForceMode.Acceleration);
        }

        //Debug.Log($"Flutter Jumping. Velocity y = {rb.velocity.y}");
        anim.SetBool(_animIDJump, true);
    }

    private void OnJumpCanceled()
    {
        //Debug.Log($"Jump canceled!");
        if (IsJumpingPrimary && !IsGrounded)
        {
            StartFall();
        }
    }

    private void EndJump()
    {
        //Debug.Log("End Jump");
        IsJumpingPrimary = false;
        anim.SetBool(_animIDJump, false);
    }

    private void EndFlutterJump()
    {
        Debug.Log("End FlutterJump");

        Instantiate(flutterSparkVfxPrefab, player.FootPos.position, Quaternion.identity);
        sparkSoundEvent?.Post(gameObject);

        if (player.Input.IsJumpPressed)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * flutterJumpFinishVelocity, ForceMode.Impulse);
        }
        isFlutterJumping = false;
        anim.SetBool(_animIDJump, false);
    }

    private void StartFall()
    {
        //Debug.Log($"Start Fall!");
        anim.SetBool(_animIDFreeFall, true);
    }

    private void AddJumpGravity()
    {
        Debug.Log($"Add Jump Gravity");
        rb.AddForce(Vector3.down * jumpGravityForce, ForceMode.Acceleration);
    }

    #endregion Jump

    #region Movement

    private bool GroundedCheck(ref bool anySlideHit)
    {
        var raycastOrigin = player.FootPos.position;
        var hits = Physics.SphereCastAll(raycastOrigin, groundDetectionRadius, Vector3.down, groundDetectionRadius,
            groundDetectionLayers, QueryTriggerInteraction.Ignore);

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Slide"))
                {
                    anySlideHit = true;
                }
                return true;
            }
        }
        return false;
    }

    private bool SlidingCheck(float minSlideAngle, bool anySlideHit)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * .6f, Vector3.down, out hit, 5f, groundDetectionLayers))
        {
            Vector3 groundNormal = hit.normal;
            float groundAngle = Vector3.Angle(groundNormal, Vector3.up);

            return groundAngle > minSlideAngle && anySlideHit;
        }
        else
        {
            return false;
        }
    }

    private SurfaceType SurfaceCheck()
    {
        Ray ray = new Ray(player.transform.position, Vector3.down);

        var hits = Physics.SphereCastAll(ray, 2f, 2f, groundDetectionLayers, QueryTriggerInteraction.Ignore);
        if (hits.Length > 0)
        {
            var tag = hits[0].collider.tag;

            switch (tag)
            {
                case "Crusher":
                case "Metal":
                    return SurfaceType.Metal;
                case "Grass":
                    return SurfaceType.Grass;
                default:
                    return SurfaceType.Stone;
            }
        }
        return SurfaceType.Stone;
    }

    private void Move(Vector3 move)
    {
        move = player.CameraPivot.TransformDirection(move);
        move.y = 0f;

        float slopeFactor = 1f;
        float currentSpeed = speed * input.MoveInput.magnitude * slopeFactor;
        Vector3 moveVelocity = move.normalized * currentSpeed;

        targetVelocity.x = moveVelocity.x;
        targetVelocity.z = moveVelocity.z;

        IsMoving = targetVelocity.magnitude > 0;
    }

    /// <summary>
    /// Apply the Final Target Velocity to Player Rigidbody
    /// </summary>
    private void ApplyMovementVelocity()
    {
        float inputMagnitude = input.MoveInput.magnitude;

        finalVelocity = targetVelocity + ExternalVelocity;

        finalVelocity.y = rb.velocity.y >= -maxFallSpeed ? rb.velocity.y : -maxFallSpeed;

        if (IsMoving)
        {
            AvoidWalls();
            rb.MovePosition(transform.position + finalVelocity * Time.deltaTime);
        }

        ApplyAnimationBlend(inputMagnitude);
    }

    private void AvoidWalls()
    {
        avoidWallsRay = new Ray(player.FirePoint.position, player.FirePoint.forward);
        if (Physics.Raycast(avoidWallsRay, wallAvoidance, groundDetectionLayers, QueryTriggerInteraction.Ignore))
        {
            var projection = Vector3.ProjectOnPlane(targetVelocity, avoidWallsRay.direction);
            finalVelocity = new Vector3(projection.x, finalVelocity.y, projection.z);
        }
    }
    #endregion

    #region Animation

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private int _animIDFlutterJumpTime;
    private Ray avoidWallsRay;

    private void ApplyAnimationBlend(float inputMagnitude)
    {
        _animationBlend = Mathf.Lerp(_animationBlend, targetVelocity.magnitude, Time.deltaTime * animationBlendSpeed);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        anim.SetFloat(_animIDSpeed, _animationBlend);
        anim.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDFlutterJumpTime = Animator.StringToHash("FlutterJumpTime");
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(avoidWallsRay);
    }
}