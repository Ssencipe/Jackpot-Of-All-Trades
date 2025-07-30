using UnityEngine;

public class CRTManager : MonoBehaviour
{
    [SerializeField] private GameObject crtEffectObject;

    private const string CRT_PREF_KEY = "CRT_ENABLED";

    private void Start()
    {
        // Always sync with stored preference on load
        bool enabled = PlayerPrefs.GetInt(CRT_PREF_KEY, 1) == 1;
        SetCRTEnabled(enabled);
    }

    public void SetCRTEnabled(bool enabled)
    {
        if (crtEffectObject != null)
            crtEffectObject.SetActive(enabled);

        // Persist across scenes
        PlayerPrefs.SetInt(CRT_PREF_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsCRTEnabled()
    {
        return crtEffectObject != null && crtEffectObject.activeSelf;
    }
}