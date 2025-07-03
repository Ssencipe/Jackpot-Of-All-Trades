using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReelVisual : MonoBehaviour
{
    [Header("Slot References")]
    [SerializeField] private List<ReelSlot> slots;

    [Header("Layout Settings")]
    [SerializeField] private float logicalSlotSpacing = 110f;
    [SerializeField] private int visibleSlotCount = 3;

    [Header("Visual Effects")]
    [SerializeField] private float visualRange = 100f;
    [SerializeField] private Vector3 centerScale = Vector3.one;
    [SerializeField] private Vector3 topScale = new Vector3(0.65f, 0.4f, 1f);
    [SerializeField] private Vector3 bottomScale = new Vector3(0.8f, 0.65f, 1f);
    [SerializeField] private Vector3 centerRotation = Vector3.zero;
    [SerializeField] private Vector3 upperRotation = new Vector3(1.5f, 0f, 0f);
    [SerializeField] private Vector3 lowerRotation = new Vector3(-0.2f, 0f, 0f);

    private RuntimeSpell[] spellPool;
    private int logicalStartIndex = 0;

    private float TopWrapY => logicalSlotSpacing * (visibleSlotCount / 2f);
    private float BottomWrapY => -logicalSlotSpacing * (visibleSlotCount / 2f);

    public void InitializeVisuals(RuntimeSpell[] spells)
    {
        spellPool = spells;
        logicalStartIndex = Random.Range(0, spellPool.Length);
        int centerIndex = visibleSlotCount / 2;

        for (int i = 0; i < slots.Count; i++)
        {
            int spellIndex = (logicalStartIndex + i) % spellPool.Length;
            RuntimeSpell spell = spellPool[spellIndex];

            slots[i].Initialize(spell, spellIndex);
            slots[i].SetLocalPosition(Vector3.zero);
            slots[i].SetIconOffsetY((centerIndex - i) * logicalSlotSpacing);
        }

        UpdateSlotVisuals();
    }

    public IEnumerator ScrollSpells(float duration, float minSpeed, float maxSpeed, RuntimeSpell[] source)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            UpdateSlotVisuals();

            float t = elapsed / duration;
            float easedT = 1 - Mathf.Pow(1 - t, 3); // EaseOutCubic
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
                    slot.Initialize(next, GetIndexInPool(next));
                }
                else if (y > TopWrapY)
                {
                    slot.OffsetIconY(-logicalSlotSpacing * slots.Count);
                    RuntimeSpell prev = GetPreviousSpell();
                    slot.Initialize(prev, GetIndexInPool(prev));
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        SnapToCenter();
    }

    private void SnapToCenter()
    {
        int centerIndex = visibleSlotCount / 2;

        for (int i = 0; i < slots.Count; i++)
        {
            float unclampedY = (centerIndex - i) * logicalSlotSpacing;
            float clampedY = Mathf.Clamp(unclampedY, BottomWrapY, TopWrapY);
            slots[i].SetIconOffsetY(clampedY);

            int spellIndex = (logicalStartIndex + i) % spellPool.Length;
            RuntimeSpell spell = spellPool[spellIndex];
            slots[i].Initialize(spell, spellIndex);
        }

        UpdateSlotVisuals();
    }

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

    public RuntimeSpell GetCenterSpell()
    {
        int centerIndex = visibleSlotCount / 2;
        return slots[centerIndex].GetSpell();
    }

    private RuntimeSpell GetNextSpell()
    {
        logicalStartIndex = (logicalStartIndex + 1) % spellPool.Length;
        return spellPool[logicalStartIndex];
    }

    private RuntimeSpell GetPreviousSpell()
    {
        logicalStartIndex = (logicalStartIndex - 1 + spellPool.Length) % spellPool.Length;
        return spellPool[logicalStartIndex];
    }

    private int GetIndexInPool(RuntimeSpell spell)
    {
        for (int i = 0; i < spellPool.Length; i++)
        {
            if (spellPool[i] == spell)
                return i;
        }
        return -1;
    }

    public List<ReelSlot> GetSlots() => slots;
}
