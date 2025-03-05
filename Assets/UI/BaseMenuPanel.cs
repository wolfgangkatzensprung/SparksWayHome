using UnityEditor;
using UnityEngine;

/// <summary>
/// Base Class for Ingame MenuPanels
/// </summary>
public class BaseMenuPanel : MonoBehaviour
{
    [Tooltip("If true, panel will not be hidden by TrySetActive")]
    public bool Persistent;

    protected virtual void OnEnable()
    {
        UIManager.Instance.AddActivePanel(this);
    }
    protected virtual void OnDisable()
    {
        UIManager.Instance?.RemoveActivePanel(this);
    }

    public bool TrySetActive(bool state)
    {
        if (!state && Persistent)
        {
            return false;
        }
        else
        {
            gameObject.SetActive(state);
            return true;
        }
    }
}
