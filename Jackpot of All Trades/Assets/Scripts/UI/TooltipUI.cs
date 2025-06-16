using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI chargeText;
    public TextMeshProUGUI colorText;
    public TextMeshProUGUI tagText;

    public Image iconImage;

    private RectTransform canvasRect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Hide();
    }

    //sets all the text and content for spell tooltip
    public void Show(SpellSO spell)
    {
        if (spell == null) return;

        titleText.text = spell.spellName;
        descriptionText.text = spell.description;

        if (iconImage != null)
            iconImage.sprite = spell.icon;

        if (chargeText != null)
        {
            if (spell.hasCharges)
                chargeText.text = $"Charge: {spell.charge}";
            else
                chargeText.text = "Charge: ∞";   //makes value infinity if spell does not use charges
        }

        if (colorText != null)
            colorText.text = $"Color: {spell.colorType}";

        if (tagText != null)
            tagText.text = $"Tags: {string.Join(", ", spell.tags)}";

        gameObject.SetActive(true);
    }

    //for status icon tooltip
    public void Show(IStatusEffect effect, Vector2 screenPos, Camera uiCamera)
    {
        if (effect == null) return;

        titleText.text = effect.ID; // You can prettify this or provide a display name
        descriptionText.text = GetDescription(effect);

        iconImage.sprite = effect.Icon;

        chargeText.text = "";  // Not applicable for statuses
        colorText.text = "";   // Not applicable for statuses
        tagText.text = $"Duration: {effect.Duration}";

        gameObject.SetActive(true);
        SetPosition(screenPos, uiCamera);
    }

    // Helper method to build readable descriptions for status icon tooltips
    private string GetDescription(IStatusEffect effect)
    {
        if (effect is OverTimeStatusInstance overTime)
        {
            return $"{overTime.Type} {overTime.Potency} each turn\n"
                 + $"Triggers at {overTime.TickTiming}";
        }

        return "Status effect applied to unit.";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    //offsets tooltip from cursor
    public void SetPosition(Vector2 screenPos, Camera uiCamera)
    {
        if (!canvasRect) return;

        // Get canvas size in screen space
        Vector2 canvasSize = canvasRect.sizeDelta;

        // Default offsets to apply directly to screen space
        float xOffset = 200f;
        float yOffset = 550f;

        // Flip X if cursor is on right side
        if (screenPos.x > Screen.width * 0.5f)
            xOffset = -xOffset;

        // Flip Y if cursor is near top
        if (screenPos.y > Screen.height * 0.6f)
            yOffset = -yOffset;

        Vector2 adjustedPos = screenPos + new Vector2(xOffset, yOffset);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, adjustedPos, uiCamera, out Vector2 localPoint))
        {
            transform.localPosition = localPoint;
        }
    }
}