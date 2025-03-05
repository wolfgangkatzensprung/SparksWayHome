using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingRoom : MonoBehaviour
{
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        var rot = Quaternion.Euler(0.0f, 15 * Time.deltaTime, 0.0f);
        rb.MoveRotation(transform.rotation * rot);
    }
}
