using UnityEngine;
using UnityEngine.EventSystems;

// Used for enemy reels

[RequireComponent(typeof(RectTransform))]
public class SpellTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [Header("Spell Data")]
    public RuntimeSpell runtimeSpell;
    public SpellSO staticSpell;

    [Header("Tooltip Placement Settings")]
    [Range(0.01f, 0.49f)]
    public float edgeThresholdPercent = 0.33f;

    private Camera uiCamera;
    private RectTransform rectTransform;

    private void Awake()
    {
        uiCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance?.Hide();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        ShowTooltip(eventData);
    }

    private void ShowTooltip(PointerEventData eventData)
    {
        if (TooltipUI.Instance == null) return;

        if (runtimeSpell != null)
        {
            TooltipUI.Instance.Show(runtimeSpell);
        }
        else if (staticSpell != null)
        {
            TooltipUI.Instance.Show(staticSpell);
        }

        TooltipUI.Instance.SetPosition(eventData.position, uiCamera);
    }
}
