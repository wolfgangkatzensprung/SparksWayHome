using Cinemachine;
using GlobalEvents;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Player Main Script that holds references of all relevant Player Components
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IActor
{
    public bool BypassSpawnPoint;
    
    [Header("Player-specific Components")]
    public PlayerInput Input;
    public PlayerInteraction Interaction;
    public PlayerMovement Movement;
    public Animator Animator;

    [Header("References: Camera and Orientation")]
    [SerializeField] CinemachineVirtualCamera vcam;
    Camera mainCam;
    public Transform CameraPivot;
    public Transform ModelTrans;

    [Header("References: Components")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform footPos;

    public Rigidbody Rigidbody { get { return rb; } }
    public Transform FirePoint { get { return firePoint; } }
    public Transform FootPos { get { return footPos; } }

    public Action OnDeath { get; internal set; }
    public Action<Player> OnRespawn { get; internal set; }

    int _collectablesAmount = 0;
    public int CollectablesAmount
    {
        get => _collectablesAmount;
        set
        {
            _collectablesAmount = value;
            GameEvents.Trigger<int>(Events.CollectableCollected, _collectablesAmount);
        }
    }

    public Checkpoint LastCheckpoint;

    private bool dead;

    private void OnEnable()
    {
        GameEvents.Connect<bool>(Events.GamePaused, OnPaused);
    }

    private void OnDisable()
    {
        GameEvents.Disconnect<bool>(Events.GamePaused, OnPaused);
    }

    private void Start()
    {
        Game.HideCursor();
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        mainCam = Camera.main;
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSecondsRealtime(3f);
        Respawn();
    }

    public void Die(DeathReason reason)
    {
        switch (reason)
        {
            case DeathReason.Water:
                Animator.SetTrigger("WaterDeath");
                break;
            case DeathReason.Fall:
                Animator.SetTrigger("FallDeath");
                break;
        }

        Die();
    }

    public void Die()
    {
        if (dead) return;

        dead = true;

        Input.enabled = false;
        Movement.enabled = false;

        rb.velocity = Vector3.zero;

        Debug.Log("Player died.");
        OnDeath?.Invoke();
        StartCoroutine(RespawnRoutine());
    }

    private void Respawn()
    {
        if (!dead) return;

        dead = false;

        Input.enabled = true;
        Movement.enabled = true;

        Animator.Play("Locomotion");

        OnRespawn?.Invoke(this);
    }

    private void OnPaused(bool state)
    {
        Input.enabled = !state;
        //Movement.enabled = !state;
    }

    internal void DestroyAll()
    {
        Destroy(vcam.gameObject);
        Destroy(Camera.main.gameObject);
        Destroy(gameObject);
    }
}
