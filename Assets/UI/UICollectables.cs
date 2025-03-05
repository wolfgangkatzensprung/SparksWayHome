using GlobalEvents;
using TMPro;
using UnityEngine;

public class UICollectables : MonoBehaviour
{
    TextMeshProUGUI collectablesText;

    private void Awake()
    {
        collectablesText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        GameEvents.Connect<int>(Events.CollectableCollected, OnCollectableCollected);
    }
    private void OnDisable()
    {
        GameEvents.Disconnect<int>(Events.CollectableCollected, OnCollectableCollected);
    }

    private void OnCollectableCollected(int newAmount)
    {
        collectablesText.text = "Collectables: " + newAmount.ToString();
    }
}
