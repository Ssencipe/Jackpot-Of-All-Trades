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
    public Image backgroundImage;

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

    //sets all the text and content for spell tooltip using SO data as backup
    public void Show(SpellSO spell)
    {
        if (spell == null) return;

        SetTitle(spell.spellName);
        SetDescription(spell.description);
        SetIcon(spell.icon);
        SetBackgroundColor(new Color(0.5f, 0.5f, 1f, 0.95f)); // Use direct RGBA value for status background

        if (spell.hasCharges)
            chargeText.text = $"Charge: {spell.charge}";
        else
            chargeText.text = "Charge: ∞"; //makes charge infinity

        colorText.text = $"Color: {spell.colorType}";
        tagText.text = $"Tags: {string.Join(", ", spell.tags)}";

        ShowUI();
    }

    //sets all the text and content for spell tooltip using runtime data
    public void Show(RuntimeSpell spell)
    {
        if (spell == null || spell.baseData == null) return;

        SetTitle(spell.spellName);
        SetDescription(spell.description);
        SetIcon(spell.icon);
        SetBackgroundColor(new Color(0.5f, 0.5f, 1f, 0.95f));

        if (spell.hasCharges)
            chargeText.text = $"Charge: {spell.charge}";
        else
            chargeText.text = "Charge: ∞";

        colorText.text = $"Color: {spell.colorType}";
        tagText.text = $"Tags: {string.Join(", ", spell.tags)}";

        ShowUI();
    }

    public void ShowUI() => gameObject.SetActive(true);

    public void Hide() => gameObject.SetActive(false);

    public void ClearSpellFields()
    {
        chargeText.text = "";
        colorText.text = "";
        tagText.text = "";
    }

    public void SetTitle(string title) => titleText.text = title;
    public void SetDescription(string desc) => descriptionText.text = desc;
    public void SetIcon(Sprite icon) => iconImage.sprite = icon;
    public void SetBackgroundColor(Color color) => backgroundImage.color = color;

    public void SetDuration(string duration)
    {
        if (tagText != null)
            tagText.text = $"Duration: {duration}";
    }

    //offsets tooltip from cursor
    public void SetPosition(Vector2 screenPos, Camera uiCamera)
    {
        if (!canvasRect) return;

        // Get canvas size in screen space
        Vector2 canvasSize = canvasRect.sizeDelta;

        // Default offsets to apply directly to screen space
        float xOffset = 200f;
        float yOffset = 300f;

        // Flip X if cursor is on right side
        if (screenPos.x > Screen.width * 0.5f)
            xOffset = -xOffset * 2f;

        // Flip Y if cursor is near top
        if (screenPos.y > Screen.height * 0.6f)
            yOffset = -yOffset * 0.5f;

        Vector2 adjustedPos = screenPos + new Vector2(xOffset, yOffset);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, adjustedPos, uiCamera, out Vector2 localPoint))
        {
            transform.localPosition = localPoint;
        }
    }
}