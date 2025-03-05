using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrushZone : MonoBehaviour
{
    Player player;
    [SerializeField] private BoxCollider boxCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
        }

        if (other.CompareTag("Crusher"))
        {
            if (player != null)
            {
                player.Die();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.center + transform.position, boxCollider.size);
    }
}
