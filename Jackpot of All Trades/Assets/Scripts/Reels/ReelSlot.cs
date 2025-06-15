using UnityEngine;
using UnityEngine.UI;

public class ReelSlot : MonoBehaviour
{
    [Header("References")]
    public Image spellIcon;

    private float currentY;
    private RectTransform iconRect;

    // The logical SpellSO this slot is currently representing
    private SpellSO spell;

    private int spellIndex;
    public void SetSpellIndex(int index) => spellIndex = index;
    public int GetSpellIndex() => spellIndex;


    public SpellSO GetSpell() => spell;
    public Sprite GetIconSprite() => spellIcon.sprite;

    private void Awake()
    {
        iconRect = spellIcon.rectTransform;
    }

    // Sets both the icon and the underlying spell data
    public void Initialize(Sprite icon, SpellSO spellRef, int index)
    {
        spell = spellRef;
        spellIcon.sprite = icon;
        spellIcon.enabled = icon != null;
        spellIndex = index;

        if (iconRect == null)
            iconRect = spellIcon.rectTransform;
    }

    // Sets local position of the slot object (used during layout)
    public void SetLocalPosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    // Sets Y offset for the icon within the slot
    public void SetIconOffsetY(float y)
    {
        currentY = y;
        UpdateIconTransform();
    }

    // Adjusts Y offset by a delta
    public void OffsetIconY(float delta)
    {
        currentY += delta;
        UpdateIconTransform();
    }

    // Returns current offset used for sorting and visual effects
    public float GetIconOffsetY() => currentY;

    // Applies offset to icon transform
    private void UpdateIconTransform()
    {
        if (iconRect == null)
            iconRect = spellIcon.rectTransform;

        iconRect.localPosition = new Vector3(0f, currentY, 0f);
    }

    // Applies scale and rotation effects to the icon
    public void SetVisuals(Vector3 scale, Quaternion rotation)
    {
        if (spellIcon == null) return;
        RectTransform iconRect = spellIcon.rectTransform;
        iconRect.localScale = scale;
        iconRect.localRotation = rotation;
    }
}