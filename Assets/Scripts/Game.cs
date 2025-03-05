using GlobalEvents;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the main game state and player data.
/// </summary>
public class Game : Singleton<Game>
{
    public static bool IsPaused { get; private set; }

    public delegate void GameLoadedDelegate();

    public GameLoadedDelegate onGameLoaded;

    public string firstScene = "MainMenu";

    [SerializeField]
    AK.Wwise.RTPC soundVolume;
    [SerializeField]
    AK.Wwise.RTPC musicVolume;

    [SerializeField]
    AK.Wwise.Event pauseEvent;
    [SerializeField]
    AK.Wwise.Event resumeEvent;

    private void Start()
    {
        LoadSettings();

        #if UNITY_EDITOR
        return;
        #endif
        SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        SceneManager.LoadScene(firstScene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Load and Apply Settings from PlayerPrefs
    /// </summary>
    private void LoadSettings()
    {
        if (PlayerPrefsHandler.TryGetFloat("SoundVolume", out var sv))
            soundVolume.SetGlobalValue(sv);
        else soundVolume.SetGlobalValue(.7f);

        if (PlayerPrefsHandler.TryGetFloat("MusicVolume", out var mv))
            musicVolume.SetGlobalValue(mv);
        else soundVolume.SetGlobalValue(.7f);

        if (PlayerPrefsHandler.TryGetBool("VsyncEnabled", out var vs))
            QualitySettings.vSyncCount = vs ? 1 : 0;
        else QualitySettings.vSyncCount = 0;

        if (PlayerPrefsHandler.TryGetBool("FullscreenEnabled", out var fs))
            Screen.fullScreen = fs;
        else Screen.fullScreen = true;
    }

    /// <summary>
    /// Disable Input. Freeze Time. Apply AudioMix Effect.
    /// </summary>
    public static void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        Instance?.pauseEvent?.Post(Instance.gameObject);
        GameEvents.Trigger<bool>(Events.GamePaused, true);
    }

    /// <summary>
    /// Unfreeze Time. Enable Input. Reset AudioMix Effect.
    /// </summary>
    public static void Unpause()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        Instance?.resumeEvent?.Post(Instance.gameObject);
        GameEvents.Trigger<bool>(Events.GamePaused, false);
    }

    public static void Pause(bool state)
    {
        if (state)
            Pause();
        else
            Unpause();
    }

    //public void StartNewGame(string saveName)
    //{
    //    foreach (Player p in players)
    //    {
    //        p.RestoreDefaults();
    //    }
    //    EventManager.Trigger(Events.NewGameStarted);
    //}

    #region Mouse Cursor
    /// <summary>
    /// Hides and Locks the Cursor for Ingame View
    /// </summary>
    internal static void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Displays and Unlocks the Cursor for Menu/UI View
    /// </summary>
    internal static void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    internal static void ToggleCursor(bool? state = null)
    {
        if (state == null)
        {
            bool cursorState = !Cursor.visible;
            Cursor.visible = cursorState;
            Cursor.lockState = cursorState ? CursorLockMode.None : CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = (bool)state;
            Cursor.lockState = (bool)state ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    #endregion
}
