using UnityEngine;
using System.Collections;

// Identical to the player reel now; only differences may be in styling or spell sources.
public class EnemyReelVisual : BaseReelVisual
{
    public override void InitializeVisuals(RuntimeSpell[] spells)
    {
        base.InitializeVisuals(spells);

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

    public override IEnumerator ScrollSpells(float duration, float minSpeed, float maxSpeed, RuntimeSpell[] spells)
    {
        if (spells != null)
            availableSpells = spells;

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

    public override int GetCurrentIndex()
    {
        return logicalStartIndex + visibleSlotCount / 2 % availableSpells.Length;
    }

    public RuntimeSpell GetTopSpell()
    {
        if (slots == null || slots.Count < 3)
            return null;

        return slots[0].GetSpell(); // top visible slot
    }

    public RuntimeSpell GetBottomSpell()
    {
        if (slots == null || slots.Count < 3)
            return null;

        return slots[slots.Count - 1].GetSpell(); // bottom visible slot
    }
}