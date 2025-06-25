using UnityEngine;

public static class DevSettings
{
    private const string DevModeKey = "DevMode";

    //saves if game was in dev mode
    public static bool IsDevMode
    {
        get => PlayerPrefs.GetInt(DevModeKey, 0) == 1;
        set
        {
            PlayerPrefs.SetInt(DevModeKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}