using System;
using UnityEngine;

public class AutoSelfDestruct : MonoBehaviour
{
    public float selfDestructionTime = 3f;

    private void Start()
    {
        Invoke("DestroySelf", selfDestructionTime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
