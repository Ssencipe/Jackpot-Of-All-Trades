using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Base class for reel visuals. Handles layout, spell indexing, and visual effects.
public abstract class BaseReelVisual : MonoBehaviour
{
    [Header("Slot References")]
    [SerializeField] protected List<ReelSlot> slots;

    [Header("Layout Settings")]
    [SerializeField] protected float logicalSlotSpacing = 110f;
    [SerializeField] protected int visibleSlotCount = 3;

    [Header("Visual Effects")]
    [SerializeField] protected float visualRange = 100f;
    [SerializeField] protected Vector3 centerScale = Vector3.one;
    [SerializeField] protected Vector3 topScale = new Vector3(0.8f, 0.65f, 1f);
    [SerializeField] protected Vector3 bottomScale = new Vector3(0.8f, 0.65f, 1f);
    [SerializeField] protected Vector3 centerRotation = Vector3.zero;
    [SerializeField] protected Vector3 upperRotation = new Vector3(0.2f, 0f, 0f);
    [SerializeField] protected Vector3 lowerRotation = new Vector3(-0.2f, 0f, 0f);

    protected RuntimeSpell[] availableSpells;
    protected int logicalStartIndex = 0;

    // Y-bounds used for wrapping visual icons when they scroll past edges
    protected float TopWrapY => logicalSlotSpacing * (visibleSlotCount / 2f);
    protected float BottomWrapY => -logicalSlotSpacing * (visibleSlotCount / 2f);

    // Initializes the reel with a list of spells to cycle through
    public virtual void InitializeVisuals(RuntimeSpell[] spells)
    {
        availableSpells = spells;
        logicalStartIndex = 0;
    }

    // Returns the spell currently shown in the center slot
    public RuntimeSpell GetCurrentSpell()
    {
        if (availableSpells == null || availableSpells.Length == 0) return null;
        int centerIndex = visibleSlotCount / 2;
        return slots[centerIndex].GetSpell();
    }

    // Returns the index into the spell list that the reel is currently centered on
    public virtual int GetCurrentIndex() => logicalStartIndex;

    // Manually sets the logical start index of the reel (wraps safely)
    public void SetCurrentIndex(int index)
    {
        if (availableSpells == null || availableSpells.Length == 0) return;
        logicalStartIndex = (index % availableSpells.Length + availableSpells.Length) % availableSpells.Length;
    }

    // Must be overridden by subclasses to animate the scrolling effect
    public virtual IEnumerator ScrollSpells(float duration, float minSpeed, float maxSpeed, RuntimeSpell[] spells)
    {
        yield break;
    }

    // Gets the next spell from the pool, cycling forward and updating the logical index
    protected RuntimeSpell GetNextSpell()
    {
        logicalStartIndex = (logicalStartIndex + 1) % availableSpells.Length;
        return availableSpells[logicalStartIndex];
    }

    // Gets the previous spell from the pool, cycling backward and updating the logical index
    protected RuntimeSpell GetPreviousSpell()
    {
        logicalStartIndex = (logicalStartIndex - 1 + availableSpells.Length) % availableSpells.Length;
        return availableSpells[logicalStartIndex];
    }

    // Returns the index of a spell in the current spell pool (used for syncing and debug)
    protected int GetIndexInPool(RuntimeSpell spell)
    {
        for (int i = 0; i < availableSpells.Length; i++)
        {
            if (availableSpells[i] == spell)
                return i;
        }
        return -1;
    }

    // Applies scaling and rotation effects to all slots based on their Y offset
    protected void UpdateSlotVisuals()
    {
        int centerIndex = visibleSlotCount / 2;

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            float distance = Mathf.Abs(slot.GetIconOffsetY());
            float t = Mathf.Clamp01(distance / visualRange);

            Vector3 scale = Vector3.Lerp(
                centerScale,
                slot.GetIconOffsetY() > 0 ? topScale : bottomScale,
                t
            );

            Vector3 rotation = Vector3.Lerp(
                centerRotation,
                slot.GetIconOffsetY() > 0 ? upperRotation : lowerRotation,
                t
            );

            slot.SetVisuals(scale, Quaternion.Euler(rotation));

            //Adjust text overlays for top/bottom vs center
            bool isTopOrBottom = i != centerIndex;
        }
    }

    // Aligns all icons to their target center positions and resets spell assignments
    protected void SnapToCenter()
    {
        int centerIndex = visibleSlotCount / 2;

        for (int i = 0; i < slots.Count; i++)
        {
            float unclampedY = (centerIndex - i) * logicalSlotSpacing;
            float clampedY = Mathf.Clamp(unclampedY, BottomWrapY, TopWrapY);
            slots[i].SetIconOffsetY(clampedY);

            int spellIndex = (logicalStartIndex + i) % availableSpells.Length;
            RuntimeSpell spell = availableSpells[spellIndex];
            slots[i].Initialize(spell, spellIndex);
        }

        UpdateSlotVisuals();
    }

    public RuntimeSpell GetCenterSpell()
    {
        int centerIndex = visibleSlotCount / 2;
        return slots != null && slots.Count > centerIndex ? slots[centerIndex].GetSpell() : null;
    }
}