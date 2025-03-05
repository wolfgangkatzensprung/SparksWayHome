using GlobalEvents;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager of the Ingame UI
/// Contains a child that is parent to all Ingame UI elements
/// </summary>
public class UIManager : Singleton<UIManager>
{
    public Canvas Canvas;

    [SerializeField] List<BaseMenuPanel> menuPanels = new();
#if UNITY_EDITOR
    public List<BaseMenuPanel> MenuPanels { get { return menuPanels; } set { menuPanels = value; } }
#endif
    Dictionary<Type, BaseMenuPanel> menuPanelsByType = new();
    List<BaseMenuPanel> activeMenuPanels = new();

    public GameObject loadingScreen;
    public Slider loadingSlider;

    public static bool InAnyMenu
    {
        get
        {
            return Instance.activeMenuPanels.Count > 0;
        }
    }

    bool isInCutscene;

    protected override void Awake()
    {
        base.Awake();

        foreach (var v in menuPanels)
        {
            menuPanelsByType.Add(v.GetType(), v);
        }
    }

    private void OnEnable()
    {
        GameEvents.Connect(Events.CutsceneStarted, OnCutsceneStarted);
        GameEvents.Connect(Events.CutsceneFinished, OnCutsceneFinished);
    }

    private void OnDisable()
    {
        GameEvents.Disconnect(Events.CutsceneStarted, OnCutsceneStarted);
        GameEvents.Disconnect(Events.CutsceneFinished, OnCutsceneFinished);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Escape"))
        {
            Debug.Log($"Escape pressed. In Any Menu = {InAnyMenu}. {activeMenuPanels.Count} active menu panels");
            if (InAnyMenu)
            {
                if (TryHideAllMenuPanels())
                {
                    Game.Unpause();
                    Game.HideCursor();
                }
            }
            else if (!isInCutscene)
            {
                var t = typeof(EscapeMenu);
                ToggleMenuPanel(t, true);
            }
        }
    }


    void OnCutsceneStarted() => isInCutscene = true;
    void OnCutsceneFinished() => isInCutscene = false;
    void InitializeNewGame() => TryHideAllMenuPanels();

    public void ToggleOptions() => ToggleMenuPanel(typeof(OptionsMenu));

    #region Loading Screen
    internal void ShowLoadingScreen() => loadingScreen.SetActive(true);
    internal void HideLoadingScreen(string _) => loadingScreen.SetActive(false);
    #endregion

    #region MenuPanels
    public void ToggleMenuPanel(Type menuType, bool? state = null)
    {
        Debug.Log($"ToggleMenuPanel for {menuType.Name} with state {state}");

        var panel = menuPanelsByType[menuType];

        bool panelEnabled = state ?? !panel.gameObject.activeSelf;

        if (panel.Persistent && !panelEnabled)
        {
            panel.gameObject.SetActive(false);
        }

        if (TryHideAllMenuPanels())
        {
            Game.Pause(panelEnabled);
            Game.ToggleCursor(panelEnabled);
            panel.gameObject.SetActive(panelEnabled);

            GameEvents.Trigger(Events.MenuToggled, panelEnabled);
        }
    }

    public bool TryHideAllMenuPanels()
    {
        bool allHidden = true;

        var activePanels = activeMenuPanels.ToArray();
        foreach (var panel in activePanels)
        {
            if (panel.TrySetActive(false))
            {
            }
            else
            {
                allHidden = false;
            }
        }
        return allHidden;
    }

    internal void AddActivePanel(BaseMenuPanel panel)
    {
        activeMenuPanels.Add(panel);
    }

    internal void RemoveActivePanel(BaseMenuPanel panel)
    {
        activeMenuPanels.Remove(panel);
    }
#if UNITY_EDITOR
    /// <summary>
    ///  Editor Button Press to add Menu Panels into the list. Can only find them on enabled GameObjects.
    /// </summary>
    public void AddMenuPanel(BaseMenuPanel v)
    {
        menuPanels.Add(v);
    }
#endif
    #endregion

}

#if UNITY_EDITOR
[CustomEditor(typeof(UIManager))]
public class UIManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UIManager uiManager = (UIManager)target;

        if (GUILayout.Button("Find Active BaseMenuPanels"))
        {
            FindAndAssignBaseMenuPanels(uiManager);
        }
    }

    private void FindAndAssignBaseMenuPanels(UIManager uiManager)
    {
        BaseMenuPanel[] baseMenuPanels = GameObject.FindObjectsOfType<BaseMenuPanel>();

        foreach (var panel in baseMenuPanels)
        {
            if (!uiManager.MenuPanels.Contains(panel))
            {
                uiManager.MenuPanels.Add(panel);
            }
        }
        EditorUtility.SetDirty(uiManager); // Mark UIManager as dirty to save changes
        Debug.Log($"{baseMenuPanels.Length} BaseMenuPanel(s) found and assigned.");
    }
}
#endif