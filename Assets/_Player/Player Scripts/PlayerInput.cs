using UnityEngine;
using UnityEngine.InputSystem;
using System;
using GlobalEvents;

/// <summary>
/// Player Input and Item Interaction Handler
/// </summary>
public class PlayerInput : MonoBehaviour
{
    public Action JumpStarted;
    public Action JumpPerformed;
    public Action JumpCanceled;
    public Action InteractStarted;
    public Action InteractPerformed;
    public Action InteractCanceled;
    public Action<int> MouseScroll;
    public Action AttackStarted;
    public Action AttackPerformed;
    public Action AttackCanceled;

    [SerializeField] InputActionAsset playerActionAsset;

    private const string playerActionMapName = "Player";
    private const string moveActionName = "Move";
    private const string jumpActionName = "Jump";
    private const string leftClickActionName = "Left Click";
    private const string rightClickActionName = "Right Click";
    private const string mouseMoveActionName = "Mouse Move";
    private const string interactActionName = "Interact";

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction leftClickAction;
    private InputAction rightClickAction;
    private InputAction mouseMoveAction;
    internal InputAction interactAction;

    public bool IsJumpPressed => jumpAction.IsPressed();
    public Vector2 MoveInput { get; private set; }
    public Vector3 MoveInputVector3 => new Vector3(MoveInput.x, 0f, MoveInput.y);
    public Vector2 MouseInput { get; private set; }
    public bool LeftClick { get { return leftClickAction.WasPerformedThisFrame(); } }
    public bool RightClick { get { return rightClickAction.WasPerformedThisFrame(); } }
    public bool AttackInput { get { return leftClickAction.IsPressed(); } }

    public bool IsInteractPressed { get { return interactAction.IsPressed(); } }


    private void Awake()
    {
        var playerActionMap = playerActionAsset.FindActionMap(playerActionMapName);
        moveAction = playerActionMap.FindAction(moveActionName);
        jumpAction = playerActionMap.FindAction(jumpActionName);
        leftClickAction = playerActionMap.FindAction(leftClickActionName);
        rightClickAction = playerActionMap.FindAction(rightClickActionName);
        mouseMoveAction = playerActionMap.FindAction(mouseMoveActionName);
        interactAction = playerActionMap.FindAction(interactActionName);

        RegisterActions();
    }

    void RegisterActions()
    {
        Debug.Log("Register Input Actions");
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        jumpAction.started += context => JumpStarted?.Invoke();
        jumpAction.performed += context => JumpPerformed?.Invoke();
        jumpAction.canceled += context => JumpCanceled?.Invoke();

        mouseMoveAction.performed += rawMouseDelta => MouseInput = rawMouseDelta.ReadValue<Vector2>();
        mouseMoveAction.canceled += rawMouseDelta => MouseInput = Vector2.zero;
        
        leftClickAction.started += context => AttackStarted?.Invoke();
        leftClickAction.performed += context => AttackPerformed?.Invoke();
        leftClickAction.canceled += context => AttackCanceled?.Invoke();

        interactAction.started += context => InteractStarted?.Invoke();
        interactAction.canceled += context => InteractCanceled?.Invoke();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        leftClickAction.Enable();
        rightClickAction.Enable();
        mouseMoveAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        leftClickAction.Disable();
        rightClickAction.Disable();
        mouseMoveAction.Disable();
        interactAction.Disable();
    }

    void Update()
    {
        HandleMouseScrollInput();
    }

    private void HandleMouseScrollInput()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            MouseScroll?.Invoke(1);
            return;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            MouseScroll?.Invoke(-1);
            return;
        }
    }
}
