using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReelSlot : MonoBehaviour
{
    [Header("References")]
    public Image spellIcon;

    [Header("Overlay Text")]
    public TextMeshProUGUI tallyText;
    public TextMeshProUGUI chargeText;

    private float currentY;
    private RectTransform iconRect;

    // Now stores the RuntimeSpell instead of SpellSO
    private RuntimeSpell spell;

    private int spellIndex;
    public void SetSpellIndex(int index) => spellIndex = index;
    public int GetSpellIndex() => spellIndex;

    public RuntimeSpell GetSpell() => spell;
    public Sprite GetIconSprite() => spellIcon.sprite;

    private void Awake()
    {
        iconRect = spellIcon.rectTransform;
    }

    // Sets both the icon and the runtime spell data
    public void Initialize(RuntimeSpell spellRef, int index)
    {
        spell = spellRef;
        spellIndex = index;

        spellIcon.sprite = spellRef?.icon;
        spellIcon.enabled = spellRef?.icon != null;

        if (iconRect == null)
        {
            iconRect = spellIcon.rectTransform;
        }

        // set color for spells modified by conditions
        spellIcon.color = SpellVisualUtil.GetColorForRuntimeSpell(spell);

        // Set overlay text values
        if (chargeText != null)
        {
            if (spellRef.hasCharges)
            {
                chargeText.text = spellRef.charge.ToString();
                chargeText.enabled = true;
            }
            else
            {
                chargeText.enabled = false;
            }
        }

        if (tallyText != null)
        {
            tallyText.text = spellRef.tally.ToString();
        }

        bool isTopOrBottom = spellIndex != 1;
        AdjustCounterOverlays(isTopOrBottom);
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

    //offset the tally and charge counters when tilted
    public void AdjustCounterOverlays(bool isTopOrBottom)
    {
        float overlayScale = isTopOrBottom ? 1.5f : 1f;
        float positionShiftX = isTopOrBottom ? 20f : 0f;
        if (chargeText != null)
        {
            chargeText.transform.localRotation = Quaternion.identity;
            chargeText.transform.localScale = Vector3.one * overlayScale;
            var original = chargeText.transform.localPosition;
            chargeText.transform.localPosition = new Vector3((chargeText.transform.localPosition.x + positionShiftX), original.y, original.z);
        }

        if (tallyText != null)
        {
            tallyText.transform.localRotation = Quaternion.identity;
            tallyText.transform.localScale = Vector3.one * overlayScale;
            var original = tallyText.transform.localPosition;
            tallyText.transform.localPosition = new Vector3((tallyText.transform.localPosition.x + positionShiftX), original.y, original.z);
        }
    }

    //hide counters for reel movement
    public void SetCountersActive(bool isActive)
    {
        if (chargeText != null)
            chargeText.enabled = isActive && spell.hasCharges;

        if (tallyText != null)
            tallyText.enabled = isActive;
    }

}