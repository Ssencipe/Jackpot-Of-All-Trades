using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusTooltipPanel : MonoBehaviour
{
    public static StatusTooltipPanel Instance { get; private set; }

    [Header("Components")]
    public TextMeshProUGUI statusNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI durationText;
    public TextMeshProUGUI timingText;
    public TextMeshProUGUI sourceText;

    public Image statusIcon;
    public Image sourceIcon;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(IStatusEffect effect)
    {
        if (effect is OverTimeStatusInstance overtime)
        {
            statusNameText.text = overtime.Label;
            descriptionText.text = $"Applies {overtime.Potency} {TooltipFormatter.FormatEffectType(overtime.Type)} each turn.";
            durationText.text = $"Duration: {overtime.Duration}";
            timingText.text = $"{TooltipFormatter.FormatTickTiming(overtime.TickTiming)}";
            sourceText.text = $"Origin: {overtime.SourceSpellName ?? "Unknown"}";

            statusIcon.sprite = overtime.Icon;
            if (sourceIcon != null)
                sourceIcon.sprite = overtime.SourceIcon;

            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetPosition(Vector3 screenPosition, Camera cam)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform, screenPosition, cam, out Vector2 localPoint);
        transform.localPosition = localPoint;
    }
}
