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

    //sets all the text and content
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
                chargeText.text = "Charge: <b>∞</b>";   //makes value infinity if spell does not use charges
        }

        if (colorText != null)
            colorText.text = $"Color: {spell.colorType}";

        if (tagText != null)
            tagText.text = $"Tags: {string.Join(", ", spell.tags)}";

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    //offsets tooltip from cursor
    public void SetPosition(Vector2 screenPos, Camera uiCamera)
    {
        if (!canvasRect) return;

        // Apply the offset directly to screen space
        screenPos += new Vector2(200f, 550f);

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCamera, out localPoint))
        {
            transform.localPosition = localPoint;
        }
    }
}