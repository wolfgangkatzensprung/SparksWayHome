using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsHandler
{
    public static Dictionary<string, object> playerPrefs = new();

    #region Bool Methods
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        if (playerPrefs.TryAdd(key, value))
        {
            playerPrefs[key] = value;
        }
    }

    public static bool GetBool(string key)
    {
        var value = PlayerPrefs.GetInt(key) == 1;
        if (playerPrefs.TryAdd(key, value))
        {
            playerPrefs[key] = value;
        }
        return value;
    }

    public static bool TryGetBool(string key, out bool value)
    {
        if (HasKey(key))
        {
            value = GetBool(key);
            return true;
        }
        else
        {
            value = false;
            return false;
        }
    }
    #endregion

    #region Int Methods
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        if (playerPrefs.TryAdd(key, value))
        {
            playerPrefs[key] = value;
        }
    }

    public static int GetInt(string key)
    {
        var value = PlayerPrefs.GetInt(key);
        if (playerPrefs.TryAdd(key, value))
        {
            playerPrefs[key] = value;
        }
        return value;
    }

    public static bool TryGetInt(string key, out int value)
    {
        if (HasKey(key))
        {
            value = GetInt(key);
            return true;
        }
        else
        {
            value = 0;
            return false;
        }
    }
    #endregion

    #region Float Methods
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        if (playerPrefs.TryAdd(key, value))
        {
            playerPrefs[key] = value;
        }
    }

    public static float GetFloat(string key)
    {
        var value = PlayerPrefs.GetFloat(key);
        if (playerPrefs.TryAdd(key, value))
        {
            playerPrefs[key] = value;
        }
        return value;
    }

    public static bool TryGetFloat(string key, out float value)
    {
        if (HasKey(key))
        {
            value = GetFloat(key);
            return true;
        }
        else
        {
            value = 0f;
            return false;
        }
    }
    #endregion

    #region String Methods
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        if (playerPrefs.TryAdd(key, value))
        {
            playerPrefs[key] = value;
        }
    }

    public static string GetString(string key)
    {
        var value = PlayerPrefs.GetString(key);
        if (playerPrefs.TryAdd(key, value))
        {
            playerPrefs[key] = value;
        }
        return value;
    }

    public static bool TryGetString(string key, out string value)
    {
        if (HasKey(key))
        {
            value = GetString(key);
            return true;
        }
        else
        {
            value = "";
            return false;
        }
    }
    #endregion

    #region Other Methods
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion
}
