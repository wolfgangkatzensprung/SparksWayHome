using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (Input.GetButtonUp("Back"))
        {
            button?.onClick?.Invoke();
        }
    }
}
