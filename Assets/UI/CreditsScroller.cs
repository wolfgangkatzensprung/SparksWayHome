using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 20f;

    private void Update()
    {
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
    }
}
