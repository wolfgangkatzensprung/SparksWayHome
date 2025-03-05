using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Checkbox : MonoBehaviour
{
    Image img;

    [SerializeField]
    Sprite enabledSprite;
    [SerializeField]
    Sprite disabledSprite;

    bool isChecked;

    public Action<bool> onToggled;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void Interact() => Interact(!isChecked);

    public void Interact(bool b)
    {
        isChecked = b;

        img.sprite = isChecked ? enabledSprite : disabledSprite;

        onToggled?.Invoke(isChecked);
    }

    public void SetWithoutNotify(bool b)
    {
        isChecked = b;

        img.sprite = isChecked ? enabledSprite : disabledSprite;
    }
}
