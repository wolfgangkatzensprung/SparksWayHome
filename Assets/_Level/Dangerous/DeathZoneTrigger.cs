using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathZoneTrigger : MonoBehaviour
{
    public DeathReason DeathReason;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            player.Die(DeathReason);
        }
    }
}

public enum DeathReason
{
    Water,
    Fall
}