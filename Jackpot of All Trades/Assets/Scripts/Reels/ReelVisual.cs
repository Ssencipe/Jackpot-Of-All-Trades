using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Player version of the reel visual, adds nudging on top of base behavior
public class ReelVisual : BaseReelVisual
{
    // Called externally to set up this reel
    public override void InitializeVisuals(RuntimeSpell[] availableSpells)
    {
        base.InitializeVisuals(availableSpells);

        logicalStartIndex = Random.Range(0, availableSpells.Length);
        int centerIndex = visibleSlotCount / 2;

        for (int i = 0; i < slots.Count; i++)
        {
            int spellIndex = (logicalStartIndex + i) % availableSpells.Length;
            RuntimeSpell spell = availableSpells[spellIndex];

            slots[i].Initialize(spell, spellIndex);
            slots[i].SetLocalPosition(Vector3.zero);
            slots[i].SetIconOffsetY((centerIndex - i) * logicalSlotSpacing);
        }

        UpdateSlotVisuals();
    }

    // Adds player-specific nudging behavior
    public IEnumerator Nudge(bool isUp, RuntimeSpell[] spellSource)
    {
        float distance = logicalSlotSpacing;
        float direction = isUp ? 1f : -1f;
        float moved = 0f;

        if (isUp)
            logicalStartIndex = (logicalStartIndex + 1) % availableSpells.Length;
        else
            logicalStartIndex = (logicalStartIndex - 1 + availableSpells.Length) % availableSpells.Length;

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

    public override IEnumerator ScrollSpells(float duration, float minSpeed, float maxSpeed, RuntimeSpell[] spellSource)
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

            foreach (var slot in slots)
            {
                float y = slot.GetIconOffsetY();

                if (y < BottomWrapY)
                {
                    slot.OffsetIconY(logicalSlotSpacing * slots.Count);
                    RuntimeSpell next = GetNextSpell();
                    int index = GetIndexInPool(next);
                    slot.Initialize(next, index);
                }
                else if (y > TopWrapY)
                {
                    slot.OffsetIconY(-logicalSlotSpacing * slots.Count);
                    RuntimeSpell prev = GetPreviousSpell();
                    int index = GetIndexInPool(prev);
                    slot.Initialize(prev, index);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        SnapToCenter();
    }

    public RuntimeSpell GetSpellAtVisualIndex(int index)
    {
        if (index < 0 || index >= slots.Count) return null;
        return slots[index].GetSpell();
    }

    public List<ReelSlot> GetSlots() => slots;
}