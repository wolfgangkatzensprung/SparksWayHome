using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(ProjectileRoutine(collision));
    }

    private IEnumerator ProjectileRoutine(Collision collision)
    {
        yield return new WaitForFixedUpdate();

        if (collision.collider.CompareTag("Player"))
        {
            if (collision.transform.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(rb.velocity, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }
}
