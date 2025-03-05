using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            player.CollectablesAmount += 1;
            Debug.Log($"Collectable {name} collected by {player.name}. CollectablesAmount is now {player.CollectablesAmount}");
            gameObject.SetActive(false);
        }
    }
}
