using UnityEngine;

public class CRTManager : MonoBehaviour
{
    [SerializeField] private GameObject crtEffectObject;

    private Transform scanlineTransform;
    private Vector3 initialScanlinePosition;

    private const string CRT_PREF_KEY = "CRT_ENABLED";

    [Header("Scanline Oscillation Settings")]
    public float verticalAmplitude = 0.25f;     // Max movement
    public float oscillationSpeed = 0.1f;      // Oscillations per second

    private void Start()
    {
        if (crtEffectObject != null && crtEffectObject.transform.childCount > 0)
        {
            scanlineTransform = crtEffectObject.transform.GetChild(0);
            initialScanlinePosition = scanlineTransform.localPosition;
        }

        // Sync with stored preference
        bool enabled = PlayerPrefs.GetInt(CRT_PREF_KEY, 1) == 1;
        SetCRTEnabled(enabled);
    }

    private void Update()
    {
        if (IsCRTEnabled() && scanlineTransform != null)
        {
            float yOffset = Mathf.Sin(Time.time * oscillationSpeed * Mathf.PI * 2f) * verticalAmplitude;
            scanlineTransform.localPosition = initialScanlinePosition + new Vector3(0f, yOffset, 0f);
        }
    }

    public void SetCRTEnabled(bool enabled)
    {
        if (crtEffectObject != null)
            crtEffectObject.SetActive(enabled);

        PlayerPrefs.SetInt(CRT_PREF_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsCRTEnabled()
    {
        return crtEffectObject != null && crtEffectObject.activeSelf;
    }
}
