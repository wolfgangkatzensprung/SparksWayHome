using System;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    public UnityEvent respawnEvent;
    public Action onActivate;

    [SerializeField]
    private bool activateOnce = false;
    private bool wasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if ((activateOnce) && (wasTriggered)) { return; }
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player.LastCheckpoint != this)
            {
                if (player.LastCheckpoint != null)
                {
                    player.LastCheckpoint.Deactivate(player);
                }

                player.LastCheckpoint = this;
                wasTriggered = true;
                Activate(player);
            }
        }
    }

    private void Activate(Player player)
    {
        player.OnRespawn += OnPlayerRespawn;
        onActivate?.Invoke();
    }

    private void Deactivate(Player player)
    {
        player.OnRespawn -= OnPlayerRespawn;
    }

    public void OnPlayerRespawn(Player player)
    {
        if (player.LastCheckpoint == this)
        {
            player.transform.SetPositionAndRotation(transform.position, transform.rotation);
            respawnEvent?.Invoke();
        }
    }

    public void ForcePlayerSpawn()
    {
        FindObjectOfType<Player>().GetComponent<Rigidbody>().position = transform.position;
    }
}
