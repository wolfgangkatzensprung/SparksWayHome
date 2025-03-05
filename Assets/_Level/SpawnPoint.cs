using System;
using System.Collections;
using UnityEngine;

public class SpawnPoint : Checkpoint
{
    Player player;
    private void Start()
    {
        player = FindObjectOfType<Player>();

#if UNITY_EDITOR
        if (player.BypassSpawnPoint) return;
#endif

        player.Rigidbody.position = transform.position;

    }
}