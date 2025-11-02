using UnityEngine;
using UnityEngine.UI;

// Manages just the lock visuals of reels

public class LockTuah : MonoBehaviour
{
    private Image lockImage;

    private void Awake()
    {
        lockImage = GetComponent<Image>();
        if (lockImage != null)
            lockImage.enabled = false; // Hide lock sprite at start
    }

    public void SetLockVisual(bool isActive)
    {
        if (lockImage != null)
            lockImage.enabled = isActive;
    }
}
