using UnityEngine;

public class DownwardRaycast : MonoBehaviour
{
    RaycastHit hit;

    private void Update()
    {
        if (!Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit))
        {
            Debug.Log("Player out of map?!");
        }
    }
}
