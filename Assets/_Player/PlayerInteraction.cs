using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Player player;

    [SerializeField] LayerMask interactableLayer;

    [SerializeField, Range(1f, 10f), Tooltip("Maximum interaction and pickup radius")]
    internal float interactionDistance = 4.2f;

    float interactionBlend
    {
        get => player.Animator.GetFloat("InteractBlend");
        set => player.Animator.SetFloat("InteractBlend", value);
    }

    float interactionBlendSpeed = 3f;

    bool _isInteracting;
    public bool IsInteracting
    {
        get => _isInteracting;
        private set
        {
            if (_isInteracting == value) return;
            _isInteracting = value;

            player.Animator.SetBool("Interact", _isInteracting);
        }
    }

    private RaycastHit interactionHit;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        player.Input.InteractStarted += OnInteractStarted;
        player.Input.InteractCanceled += OnInteractCanceled;
    }

    private void OnDisable()
    {
        player.Input.InteractStarted -= OnInteractStarted;
        player.Input.InteractCanceled -= OnInteractCanceled;
    }

    private void Update()
    {
        if (IsInteracting)
        {
            HoldInteract();
        }
    }

    #region Interaction
    private void OnInteractStarted()
    {
        IsInteracting = FindInteractable() != null;
    }

    private void HoldInteract()
    {
        SetInteractionBlend(1f, interactionBlendSpeed * Time.deltaTime);
    }

    private IInteractable FindInteractable()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, interactionDistance, Vector3.forward, interactionDistance, interactableLayer, QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.Interact(player))
                {
                    Debug.Log($"Interacted with {interactable}.");
                }
                else
                {
                    Debug.Log($"Tried interacting with {interactable} but didn't work.");
                }

                interactionHit = hit;
                return interactable;
            }
            else
            {
                Debug.Log("No interactable found. Try find child interactable...");

                var childInteractables = hit.transform.GetComponentsInChildren<IInteractable>();

                foreach (var childInteractable in childInteractables)
                {
                    if (Vector3.Distance(transform.position, childInteractable.GetTransform().position) <= interactionDistance)
                    {
                        Debug.Log($"Interacting with child interactable: {childInteractable.GetTransform().name}");
                        childInteractable.Interact(player);
                        interactionHit = hit;
                        return childInteractable;
                    }
                }
            }
        }

        return null;
    }

    private void SetInteractionBlend(float targetBlendValue, float blendSpeed)
    {
        interactionBlend = Mathf.MoveTowards(interactionBlend, targetBlendValue, blendSpeed);
    }

    private void OnInteractCanceled()
    {
        if (!IsInteracting) return;

        Debug.Log($"Finished hold interaction with {interactionHit.transform.name}");
        IsInteracting = false;
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (player.Input.interactAction != null && player.Input.IsInteractPressed)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
    }
}
