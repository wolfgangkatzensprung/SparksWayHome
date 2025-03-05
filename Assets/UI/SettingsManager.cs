using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] Slider soundSlider;

    [SerializeField] Slider musicSlider;

    [SerializeField] Checkbox vsyncCheckbox;

    [SerializeField] Checkbox fullscreenCheckbox;

    [SerializeField]
    AK.Wwise.RTPC soundVolume;
    [SerializeField]
    AK.Wwise.RTPC musicVolume;

    private void OnEnable()
    {
        soundSlider.onValueChanged.AddListener(SetSoundVolume);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);

        vsyncCheckbox.onToggled += OnVsyncCheckboxToggled;

        fullscreenCheckbox.onToggled += OnFullscreenCheckboxToggled;
    }

    private void OnDisable()
    {
        soundSlider.onValueChanged.RemoveListener(SetSoundVolume);

        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);

        vsyncCheckbox.onToggled -= OnVsyncCheckboxToggled;

        fullscreenCheckbox.onToggled -= OnFullscreenCheckboxToggled;
    }

    private void Start()
    {
        LoadSettingsUI();
    }

    /// <summary>
    /// Load Settings Checkboxes and Sliders without Notify
    /// </summary>
    private void LoadSettingsUI()
    {
        soundSlider.SetValueWithoutNotify(PlayerPrefsHandler.TryGetFloat("SoundVolume", out float soundVolume) ? soundVolume : SetDefaultFloat("SoundVolume", .7f));

        musicSlider.SetValueWithoutNotify(PlayerPrefsHandler.TryGetFloat("MusicVolume", out float musicVolume) ? musicVolume : SetDefaultFloat("MusicVolume", .7f));

        vsyncCheckbox.SetWithoutNotify(PlayerPrefsHandler.TryGetBool("VsyncEnabled", out bool vsyncEnabled) ? vsyncEnabled : SetDefaultBool("VsyncEnabled", false));
        fullscreenCheckbox.SetWithoutNotify(PlayerPrefsHandler.TryGetBool("FullscreenEnabled", out bool fullscreenEnabled) ? fullscreenEnabled : SetDefaultBool("FullscreenEnabled", false));
    }

    private bool SetDefaultBool(string key, bool defaultValue)
    {
        PlayerPrefsHandler.SetBool(key, defaultValue);
        return defaultValue;
    }

    private float SetDefaultFloat(string key, float defaultValue)
    {
        PlayerPrefsHandler.SetFloat(key, defaultValue);
        return defaultValue;
    }

    private void SetSoundVolume(float v)
    {
        soundVolume.SetGlobalValue(v);
        PlayerPrefsHandler.SetFloat("SoundVolume", v);
    }

    private void SetMusicVolume(float v)
    {
        Debug.Log($"Music Volume: {v}");
        musicVolume.SetGlobalValue(v);
        Debug.Log($"Wwise Music Volume: {musicVolume.GetGlobalValue()}");
        PlayerPrefsHandler.SetFloat("MusicVolume", v);
        Debug.Log($"PlayerPrefs Music Volume: {PlayerPrefsHandler.GetFloat("MusicVolume")}");
    }

    private void OnVsyncCheckboxToggled(bool b)
    {
        PlayerPrefsHandler.SetBool("VsyncEnabled", b);
        QualitySettings.vSyncCount = b ? 1 : 0;
    }

    private void OnFullscreenCheckboxToggled(bool b)
    {
        PlayerPrefsHandler.SetBool("FullscreenEnabled", b);
        Screen.fullScreen = b;
    }
}
