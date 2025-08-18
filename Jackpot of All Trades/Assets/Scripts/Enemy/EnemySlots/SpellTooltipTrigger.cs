using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SpellTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [Header("References")]
    public EnemyReel reel;

    [Header("Tooltip Thresholds")]
    [Range(0.01f, 0.49f)]
    public float edgeThresholdPercent = 0.33f;

    private RectTransform rectTransform;
    private Camera uiCamera;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        uiCamera = Camera.main;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        ShowTooltip(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance?.Hide();
    }

    private void ShowTooltip(PointerEventData eventData)
    {
        if (reel == null || TooltipUI.Instance == null || rectTransform == null)
            return;

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, uiCamera, out localPoint))
            return;

        float normalizedY = Mathf.InverseLerp(rectTransform.rect.yMin, rectTransform.rect.yMax, localPoint.y);

        RuntimeSpell spell = null;

        if (normalizedY >= (1f - edgeThresholdPercent))
        {
            spell = reel.GetTopSpell();
        }
        else if (normalizedY <= edgeThresholdPercent)
        {
            spell = reel.GetBottomSpell();
        }
        else
        {
            spell = reel.GetCenterSpell();
        }

        if (spell != null)
        {
            TooltipUI.Instance.Show(spell);
            TooltipUI.Instance.SetPosition(eventData.position, uiCamera);
        }
    }
}
