using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour
{
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (Input.GetButtonUp("Skip"))
        {
            button?.onClick?.Invoke();
        }
    }
}
