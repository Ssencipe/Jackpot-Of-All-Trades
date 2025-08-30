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

    private RectTransform canvasRect;

    private void Awake()
    {
        Instance = this;
        canvasRect = GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
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
        if (canvasRect == null) return;

        // Get canvas size in screen space
        Vector2 canvasSize = canvasRect.sizeDelta;

        // Offset values
        float xOffset = 200f;
        float yOffset = 300f;

        // Adjust for screen edges
        bool isRightSide = screenPosition.x > Screen.width * 0.5f;
        bool isTopSide = screenPosition.y > Screen.height * 0.6f;

        if (isRightSide)
            xOffset = -650f;
        else
            xOffset = 325f;

        if (isTopSide)
            yOffset = -150f;
        else
            yOffset = 500f;

        Vector2 adjustedPos = screenPosition + new Vector3(xOffset, yOffset);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, adjustedPos, cam, out Vector2 localPoint))
        {
            transform.localPosition = localPoint;
        }
    }
}