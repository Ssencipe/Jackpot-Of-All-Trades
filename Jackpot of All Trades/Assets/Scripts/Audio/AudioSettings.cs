using System.Collections.Generic;
using UnityEngine;

public static class AudioSettings
{
    private const string MasterKey = "Volume_Master";

    //audio category names
    private static Dictionary<AudioCategory, string> keys = new Dictionary<AudioCategory, string>
    {
        { AudioCategory.Music, "Volume_Music" },
        { AudioCategory.SFX, "Volume_SFX" },
        { AudioCategory.UI, "Volume_UI" }
    };

    //audio category volumes
    private static Dictionary<AudioCategory, float> volumeLevels = new Dictionary<AudioCategory, float>
    {
        { AudioCategory.Music, 1f },
        { AudioCategory.SFX, 1f },
        { AudioCategory.UI, 1f }
    };

    public static void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat(MasterKey, volume);
        PlayerPrefs.Save();
    }

    //big man changing all audio values
    public static float GetMasterVolume()
    {
        return PlayerPrefs.HasKey(MasterKey) ? PlayerPrefs.GetFloat(MasterKey) : 1f;
    }

    //general setter
    public static void SetVolume(AudioCategory category, float volume)
    {
        if (!keys.ContainsKey(category)) return;

        volumeLevels[category] = volume;
        PlayerPrefs.SetFloat(keys[category], volume);
        PlayerPrefs.Save();
    }

    //general getter accounting for master volume (not raw)
    public static float GetVolume(AudioCategory category)
    {
        if (!keys.ContainsKey(category)) return 1f;

        if (PlayerPrefs.HasKey(keys[category]))
            volumeLevels[category] = PlayerPrefs.GetFloat(keys[category]);

        float categoryVolume = volumeLevels[category];
        float masterVolume = GetMasterVolume();

        return categoryVolume * masterVolume;
    }

    //gets raw volume value of a category
    public static float GetRawCategoryVolume(AudioCategory category)
    {
        if (!keys.ContainsKey(category)) return 1f;

        if (PlayerPrefs.HasKey(keys[category]))
            volumeLevels[category] = PlayerPrefs.GetFloat(keys[category]);

        return volumeLevels[category];
    }
}