using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ReelClickRegion : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Reel reel;
    private LockManager lockManager;
    private NudgeManager nudgeManager;

    [Range(0.01f, 0.49f)]
    public float edgeThresholdPercent = 0.33f;

    public void SetLockManager(LockManager manager)
    {
        lockManager = manager;
    }

    public void SetNudgeManager(NudgeManager manager)
    {
        nudgeManager = manager;
    }

    public void SetReel(Reel targetReel)
    {
        reel = targetReel;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (reel == null || reel.IsSpinning()) return;

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out localPoint
        );

        float normalizedY = Mathf.InverseLerp(rectTransform.rect.yMin, rectTransform.rect.yMax, localPoint.y);

        if (normalizedY >= (1f - edgeThresholdPercent))
        {
            // Top area = Nudge up
            if (!reel.IsLocked && nudgeManager != null)
                nudgeManager.TryNudge(reel, true);
        }
        else if (normalizedY <= edgeThresholdPercent)
        {
            // Bottom area = Nudge down
            if (!reel.IsLocked && nudgeManager != null)
                nudgeManager.TryNudge(reel, false);
        }
        else
        {
            // Center = toggle lock/unlock via manager
            if (lockManager != null)
                lockManager.ToggleLock(reel);
            else
            {
                if (reel.IsLocked) reel.Unlock();
                else reel.Lock();
            }
        }
    }
}