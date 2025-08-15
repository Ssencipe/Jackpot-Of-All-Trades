using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [System.Serializable]
    public class AudioEntry
    {
        public string name;
        public AudioClip clip;
        public AudioCategory category;  //for categorizing audio for volume settings
        [Range(0f, 1f)] public float individualVolume = 1f; //for tuning audio for individual clips for balancing
    }

    public List<AudioEntry> clips;

    private Dictionary<string, AudioEntry> lookup;

    private void InitializeLookup()
    {
        if (lookup == null)
            lookup = new Dictionary<string, AudioEntry>();
        else
            lookup.Clear();

        foreach (var entry in clips)
        {
            if (!lookup.ContainsKey(entry.name))
                lookup.Add(entry.name, entry);
        }
    }

    //gets audio clip from associated string when using string to reference
    public AudioClip GetClip(string name)
    {
        InitializeLookup();
        return lookup.TryGetValue(name, out var entry) ? entry.clip : null;
    }

    public bool TryGetEntry(string name, out AudioEntry entry)
    {
        InitializeLookup();
        return lookup.TryGetValue(name, out entry);
    }
}