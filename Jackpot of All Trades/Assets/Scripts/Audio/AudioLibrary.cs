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
    }

    public List<AudioEntry> clips;

    private Dictionary<string, AudioEntry> lookup;

    private void OnEnable()
    {
        if (lookup == null || lookup.Count != clips.Count)
        {
            lookup = new Dictionary<string, AudioEntry>();
            foreach (var entry in clips)
            {
                if (!lookup.ContainsKey(entry.name))
                    lookup.Add(entry.name, entry);
            }
        }
    }

    //gets audio clip from associated string when using string to reference
    public AudioClip GetClip(string name)
    {
        if (lookup == null) OnEnable();
        return lookup.TryGetValue(name, out var entry) ? entry.clip : null;
    }

    public bool TryGetEntry(string name, out AudioEntry entry)
    {
        if (lookup == null) OnEnable();
        return lookup.TryGetValue(name, out entry);
    }
}