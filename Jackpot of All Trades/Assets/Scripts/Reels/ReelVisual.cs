using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Drives the spinning and layout logic for a visual reel of spells
public class ReelVisual : MonoBehaviour
{
    [Header("Slot References")]
    [SerializeField] private List<ReelSlot> slots; // References to the visual slot objects

    [Header("Layout Settings")]
    [SerializeField] private float logicalSlotSpacing = 110f; // Distance between visual slots
    [SerializeField] private int visibleSlotCount = 3; // Number of spell icons shown at once

    [Header("Visual Effects")]
    [SerializeField] private float visualRange = 100f; // Distance for max visual effect
    [SerializeField] private Vector3 centerScale = Vector3.one;
    [SerializeField] private Vector3 topScale = new Vector3(0.65f, 0.4f, 1f);
    [SerializeField] private Vector3 bottomScale = new Vector3(0.8f, 0.65f, 1f);
    [SerializeField] private Vector3 centerRotation = Vector3.zero;
    [SerializeField] private Vector3 upperRotation = new Vector3(1.5f, 0f, 0f);
    [SerializeField] private Vector3 lowerRotation = new Vector3(-0.2f, 0f, 0f);

    // Full list of spells that this reel will cycle through
    private SpellSO[] spellPool;

    // Logical index in spellPool representing the center of the reel
    private int logicalStartIndex = 0;

    private float TopWrapY => logicalSlotSpacing * (visibleSlotCount / 2f);
    private float BottomWrapY => -logicalSlotSpacing * (visibleSlotCount / 2f);

    // Exposes the slot list to external systems (e.g., Reel)
    public List<ReelSlot> GetSlots() => slots;

    // Called externally to set up this reel
    public void InitializeVisuals(SpellSO[] availableSpells)
    {
        spellPool = availableSpells;
        logicalStartIndex = Random.Range(0, spellPool.Length); // Start from a random spell

        int centerIndex = visibleSlotCount / 2;

        for (int i = 0; i < slots.Count; i++)
        {
            int spellIndex = (logicalStartIndex + i) % spellPool.Length;
            SpellSO spell = spellPool[spellIndex];

            slots[i].Initialize(spell.icon, spell, spellIndex);
            slots[i].SetLocalPosition(Vector3.zero);
            slots[i].SetIconOffsetY((centerIndex - i) * logicalSlotSpacing);
        }

        UpdateSlotVisuals();
    }

    // Handles automated spinning animation with visual wrapping
    public IEnumerator ScrollSpells(float duration, float minSpeed, float maxSpeed, SpellSO[] spellSource)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            UpdateSlotVisuals();

            float t = elapsed / duration;
            float easedT = Easing.EaseOutCubic(t);
            float movement = Mathf.Lerp(minSpeed, maxSpeed, easedT) * Time.deltaTime;

            foreach (var slot in slots)
                slot.OffsetIconY(-movement);

            // Wrap slots that move beyond boundaries
            foreach (var slot in slots)
            {
                float y = slot.GetIconOffsetY();

                if (y < BottomWrapY)
                {
                    slot.OffsetIconY(logicalSlotSpacing * slots.Count);
                    SpellSO next = GetNextSpell();
                    int index = GetIndexInPool(next);
                    slot.Initialize(next.icon, next, index);
                }
                else if (y > TopWrapY)
                {
                    slot.OffsetIconY(-logicalSlotSpacing * slots.Count);
                    SpellSO prev = GetPreviousSpell();
                    int index = GetIndexInPool(prev);
                    slot.Initialize(prev.icon, prev, index);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        SnapToCenter();
    }

    // Gets index of a spell in the full pool (used for debug or sync)
    private int GetIndexInPool(SpellSO spell)
    {
        for (int i = 0; i < spellPool.Length; i++)
        {
            if (spellPool[i] == spell)
                return i;
        }
        return -1;
    }

    // Scrolls the reel one slot up or down via player nudge
    public IEnumerator Nudge(bool isUp, SpellSO[] spellSource)
    {
        float distance = logicalSlotSpacing;
        float direction = isUp ? 1f : -1f;
        float moved = 0f;

        // Advance logical index before animation
        if (isUp)
            logicalStartIndex = (logicalStartIndex + 1) % spellPool.Length;
        else
            logicalStartIndex = (logicalStartIndex - 1 + spellPool.Length) % spellPool.Length;

        // Bounce easing for nudge movement
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = Easing.EaseOutBounce(t);
            float currentOffset = easedT * distance;

            float offset = currentOffset - moved;
            moved = currentOffset;

            foreach (var slot in slots)
                slot.OffsetIconY(offset * direction);

            UpdateSlotVisuals();
            elapsed += Time.deltaTime;
            yield return null;
        }

        SnapToCenter();
    }

    // Snaps each slot exactly back to their expected center positions
    private void SnapToCenter()
    {
        int centerIndex = visibleSlotCount / 2;

        for (int i = 0; i < slots.Count; i++)
        {
            float unclampedY = (centerIndex - i) * logicalSlotSpacing;
            float clampedY = Mathf.Clamp(unclampedY, BottomWrapY, TopWrapY);
            slots[i].SetIconOffsetY(clampedY);

            int spellIndex = (logicalStartIndex + i) % spellPool.Length;
            SpellSO spell = spellPool[spellIndex];
            slots[i].Initialize(spell.icon, spell, spellIndex);
        }

        UpdateSlotVisuals();
    }

    // Returns the spell currently displayed at the visual center
    public SpellSO GetCenterSpell()
    {
        int centerIndex = visibleSlotCount / 2;
        return slots[centerIndex].GetSpell();
    }

    // Applies scale/rotation effects based on distance from visual center
    private void UpdateSlotVisuals()
    {
        foreach (var slot in slots)
        {
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
        }
    }

    // Get the spell shown at a specific index in the visual slot list
    public SpellSO GetSpellAtVisualIndex(int index)
    {
        if (index < 0 || index >= slots.Count) return null;
        return slots[index].GetSpell();
    }

    // Rotates logical position forward in spell list and returns next spell
    private SpellSO GetNextSpell()
    {
        logicalStartIndex = (logicalStartIndex + 1) % spellPool.Length;
        return spellPool[logicalStartIndex];
    }

    // Rotates logical position backward in spell list and returns previous spell
    private SpellSO GetPreviousSpell()
    {
        logicalStartIndex = (logicalStartIndex - 1 + spellPool.Length) % spellPool.Length;
        return spellPool[logicalStartIndex];
    }
}