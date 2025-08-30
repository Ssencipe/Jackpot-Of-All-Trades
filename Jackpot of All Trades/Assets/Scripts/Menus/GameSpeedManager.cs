using UnityEngine;

public static class GameSpeedManager
{
    public static float CurrentSpeed
    {
        get => PlayerPrefs.GetFloat("GAME_SPEED", 1f);
        set
        {
            PlayerPrefs.SetFloat("GAME_SPEED", value);
            PlayerPrefs.Save();
            Time.timeScale = value;
        }
    }
}