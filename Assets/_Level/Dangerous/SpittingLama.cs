using UnityEngine;

public class SpittingLama : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 40f;
    bool hasShot;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasShot)
        {
            Debug.Log($"Spit");
            hasShot = true;
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                var distance = Vector3.Distance(playerRigidbody.position, firePoint.position);
                var heightOffset = Mathf.Lerp(0f, 3f, distance) + 2f;
                Quaternion aimRotation = Quaternion.LookRotation(playerRigidbody.position + Vector3.up * heightOffset - firePoint.position);
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, aimRotation);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.velocity = (playerRigidbody.position - firePoint.position).normalized * projectileSpeed;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasShot = false;
        }
    }

    private void OnDrawGizmos()
    {
    }
}
