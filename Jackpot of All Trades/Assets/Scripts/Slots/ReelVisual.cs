using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ReelVisual : MonoBehaviour
{
    // References to visual slots that display each spell
    [Header("Slot References")]
    [SerializeField] private List<ReelSlot> slots;

    // Distance between icons and number of visible icons at a time
    [Header("Layout Settings")]
    [SerializeField] private float logicalSlotSpacing = 110f;
    [SerializeField] private int visibleSlotCount = 3;

    // Visual effect parameters for scaling and rotation
    [Header("Visual Effects")]
    [SerializeField] private float visualRange = 100f;
    [SerializeField] private Vector3 centerScale = Vector3.one;
    [SerializeField] private Vector3 topScale = new Vector3(0.65f, 0.4f, 1f);
    [SerializeField] private Vector3 bottomScale = new Vector3(0.8f, 0.65f, 1f);
    [SerializeField] private Vector3 centerRotation = Vector3.zero;
    [SerializeField] private Vector3 upperRotation = new Vector3(1.5f, 0f, 0f);
    [SerializeField] private Vector3 lowerRotation = new Vector3(-0.2f, 0f, 0f);

    // Logical representation of the spell list in the reel
    private Queue<SpellSO> spellQueue = new Queue<SpellSO>();

    // Thresholds for wrapping icons visually
    private float TopWrapY => logicalSlotSpacing * (visibleSlotCount / 2f);
    private float BottomWrapY => -logicalSlotSpacing * (visibleSlotCount / 2f);

    public List<ReelSlot> GetSlots() => slots;

    // Initializes the visual slots with a randomized start position
    public void InitializeVisuals(SpellSO[] availableSpells)
    {
        spellQueue.Clear();
        int start = Random.Range(0, availableSpells.Length);

        // Fill queue with rotated order starting at random index
        for (int i = 0; i < availableSpells.Length; i++)
            spellQueue.Enqueue(availableSpells[(start + i) % availableSpells.Length]);

        int centerIndex = visibleSlotCount / 2;
        SpellSO[] queueArray = spellQueue.ToArray();

        // Position each slot and initialize its sprite and data
        for (int i = 0; i < slots.Count; i++)
        {
            SpellSO spell = queueArray[i];
            slots[i].Initialize(spell.icon, spell);
            slots[i].SetLocalPosition(Vector3.zero);
            slots[i].SetIconOffsetY((centerIndex - i) * logicalSlotSpacing);
        }

        UpdateSlotVisuals();
    }

    // Handles continuous scroll animation for spinning reels
    public IEnumerator ScrollSpells(float duration, float minSpeed, float maxSpeed, SpellSO[] spellSource)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            UpdateSlotVisuals();
            float t = elapsed / duration;
            float easedT = 1 - Mathf.Pow(1 - t, 3);
            float movement = Mathf.Lerp(minSpeed, maxSpeed, easedT) * Time.deltaTime;

            foreach (var slot in slots)
                slot.OffsetIconY(-movement);

            // Wrap icons that move beyond visual range and recycle spells
            foreach (var slot in slots)
            {
                float y = slot.GetIconOffsetY();
                if (y < BottomWrapY)
                {
                    slot.OffsetIconY(logicalSlotSpacing * slots.Count);
                    SpellSO next = GetNextSpell(spellSource);
                    slot.Initialize(next.icon, next);
                }
                else if (y > TopWrapY)
                {
                    slot.OffsetIconY(-logicalSlotSpacing * slots.Count);
                    SpellSO prev = GetPreviousSpell();
                    slot.Initialize(prev.icon, prev);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        SnapToCenter();
    }

    // Moves the reel one slot in the given direction
    public IEnumerator Nudge(bool isUp, SpellSO[] spellSource)
    {
        float distance = logicalSlotSpacing;
        float speed = 500f;
        float direction = isUp ? 1f : -1f;
        float moved = 0f;

        // Update the queue order based on nudge direction
        if (isUp)
        {
            spellQueue = new Queue<SpellSO>(
                spellQueue.Skip(1).Concat(spellQueue.Take(1))
            );
        }
        else
        {
            SpellSO[] arr = spellQueue.ToArray();
            SpellSO last = arr[arr.Length - 1];
            spellQueue = new Queue<SpellSO>(
                new[] { last }.Concat(arr.Take(arr.Length - 1))
            );
        }

        // Animate icon movement visually
        while (moved < distance)
        {
            float step = direction * speed * Time.deltaTime;
            float remaining = distance - moved;

            if (Mathf.Abs(step) > remaining)
                step = direction * remaining;

            foreach (var slot in slots)
                slot.OffsetIconY(step);

            moved += Mathf.Abs(step);

            UpdateSlotVisuals();
            yield return null;
        }

        SnapToCenter();
    }

    // Repositions icons to be perfectly aligned to logical center
    private void SnapToCenter()
    {
        int centerIndex = visibleSlotCount / 2;

        for (int i = 0; i < slots.Count; i++)
        {
            float unclampedY = (centerIndex - i) * logicalSlotSpacing;
            float clampedY = Mathf.Clamp(unclampedY, BottomWrapY, TopWrapY);
            slots[i].SetIconOffsetY(clampedY);

            SpellSO spell = spellQueue.ElementAt(i % spellQueue.Count);
            slots[i].Initialize(spell.icon, spell);
        }

        UpdateSlotVisuals();
    }

    // Dequeues next spell and cycles it to the end
    private SpellSO GetNextSpell(SpellSO[] source)
    {
        if (source == null || source.Length == 0) return null;
        SpellSO spell = spellQueue.Dequeue();
        spellQueue.Enqueue(spell);
        return spell;
    }

    // Moves the last spell to the front of the queue
    private SpellSO GetPreviousSpell()
    {
        if (spellQueue.Count == 0) return null;
        SpellSO[] arr = spellQueue.ToArray();
        SpellSO last = arr[arr.Length - 1];
        spellQueue = new Queue<SpellSO>(new[] { last }.Concat(arr.Take(arr.Length - 1)));
        return last;
    }

    // Returns the spell currently at the visual center
    public SpellSO GetCenterSpell()
    {
        int centerIndex = visibleSlotCount / 2;
        return slots[centerIndex].GetSpell();
    }

    // Applies scaling and rotation effects to each icon based on distance from center
    private void UpdateSlotVisuals()
    {
        foreach (var slot in slots)
        {
            float distance = Mathf.Abs(slot.GetIconOffsetY());
            float t = Mathf.Clamp01(distance / visualRange);

            //set scaling dor top and bottom
            Vector3 scale = Vector3.Lerp(
                centerScale,
                slot.GetIconOffsetY() > 0 ? topScale : bottomScale,
                t
            );

            //set tilting for top and bottom
            Vector3 rotation = Vector3.Lerp(
                centerRotation,
                slot.GetIconOffsetY() > 0 ? upperRotation : lowerRotation,
                t
            );

            slot.SetVisuals(scale, Quaternion.Euler(rotation));
        }
    }

    //
    public SpellSO GetSpellAtVisualIndex(int index)
    {
        if (index < 0 || index >= slots.Count) return null;
        return slots[index].GetSpell();
    }
}