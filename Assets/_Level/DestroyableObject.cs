using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    public GameObject prefabToSpawn;

    public void TriggerDestruction()
    {
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
