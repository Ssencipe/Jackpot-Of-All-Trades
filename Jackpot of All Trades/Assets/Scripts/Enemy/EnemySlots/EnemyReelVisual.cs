using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyReelVisual : MonoBehaviour
{
    [Header("Visual Components")]
    public GameObject[] spriteObjects = new GameObject[5]; // Changed from 7 to 5

    [Header("Animation Settings")]
    public float itemHeight = 100f;
    public AnimationCurve spinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Image[] sprites = new Image[5]; // Changed from 7 to 5
    private RectTransform[] rects = new RectTransform[5]; // Changed from 7 to 5
    private RuntimeSpell[] availableSpells;
    private int currentIndex = 0;

    void Awake()
    {
        // Get components for all 5 sprites
        for (int i = 0; i < spriteObjects.Length; i++)
        {
            if (spriteObjects[i] != null)
            {
                sprites[i] = spriteObjects[i].GetComponent<Image>();
                rects[i] = spriteObjects[i].GetComponent<RectTransform>();
                
                // Debug check
                Debug.Log($"Sprite {i}: {spriteObjects[i].name}, Position: {rects[i].anchoredPosition}");
            }
        }
    }

    public void InitializeVisuals(RuntimeSpell[] spells)
    {
        availableSpells = spells;
        if (spells == null || spells.Length == 0) return;

        // Set initial position
        UpdateStaticVisuals(0);
    }

    public IEnumerator ScrollSpells(float duration, float minSpeed, float maxSpeed, RuntimeSpell[] spells)
    {
        if (spells == null || spells.Length == 0) yield break;

        availableSpells = spells;
        float elapsedTime = 0f;
        float startPosition = 0f;

        // Calculate total distance for free spinning (don't target specific spell during spin)
        int extraRotations = Random.Range(3, 6);
        float totalDistance = extraRotations * spells.Length * itemHeight;
        
        // Choose final spell (but don't enforce it during animation)
        int finalIndex = Random.Range(0, spells.Length);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);

            // Apply easing curve
            float easedProgress = spinCurve.Evaluate(progress);

            // FREE SPINNING: Just calculate continuous position without targeting final spell
            float currentPosition = startPosition + (totalDistance * easedProgress);

            // Update visual position - this creates the free spinning effect
            UpdateScrollPosition(currentPosition);

            yield return null;
        }

        // SNAP TO FINAL POSITION: Only now do we set the exact final result
        currentIndex = finalIndex;
        UpdateStaticVisuals(currentIndex);

        // Reset to clean positions
        ResetPosition();
    }

    private void UpdateScrollPosition(float scrollPosition)
    {
        if (availableSpells == null || availableSpells.Length == 0) return;

        // Calculate continuous position without modulo jumps
        float spellSteps = scrollPosition / itemHeight;
        float fractionalStep = spellSteps - Mathf.Floor(spellSteps);
        
        // Calculate base spell index - use continuous calculation
        int baseSpellIndex = Mathf.FloorToInt(spellSteps);
        
        // Position offset for smooth movement
        float offset = -fractionalStep * itemHeight;

        // Update ALL 5 sprites for continuous flow
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null && rects[i] != null)
            {
                // Calculate spell index - handle wrapping properly (center is now sprite 2)
                int spellIndex = baseSpellIndex + i - 2;
                
                // Ensure positive index before modulo
                while (spellIndex < 0) 
                    spellIndex += availableSpells.Length;
                spellIndex = spellIndex % availableSpells.Length;

                // Update the sprite image
                UpdateSpriteImage(sprites[i], availableSpells[spellIndex]);

                // Calculate position - sprite 2 is center
                Vector3 pos = rects[i].anchoredPosition;
                pos.y = (2 - i) * itemHeight + offset; // Changed positioning for 5 sprites
                rects[i].anchoredPosition = pos;

                // Make sure sprite is enabled
                sprites[i].enabled = true;
                sprites[i].gameObject.SetActive(true);
            }
        }

        // Update current index properly
        currentIndex = (baseSpellIndex % availableSpells.Length + availableSpells.Length) % availableSpells.Length;
    }

    private void UpdateStaticVisuals(int centerIndex)
    {
        if (availableSpells == null || availableSpells.Length == 0) return;

        // Update all 5 sprites
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
            {
                int spellIndex = (centerIndex + i - 2 + availableSpells.Length) % availableSpells.Length;
                UpdateSpriteImage(sprites[i], availableSpells[spellIndex]);
            }
        }

        currentIndex = centerIndex;
    }

    private void UpdateSpriteImage(Image imageComponent, RuntimeSpell spell)
    {
        if (imageComponent == null || spell == null) return;

        // Use spell.icon instead of spell.spellIcon (matching your existing code)
        if (spell.icon != null)
        {
            imageComponent.sprite = spell.icon;
            imageComponent.color = Color.white;
        }
        else
        {
            imageComponent.sprite = null;
            imageComponent.color = Color.clear;
        }
    }

    private void ResetPosition()
    {
        // Reset all 5 sprite positions
        for (int i = 0; i < rects.Length; i++)
        {
            if (rects[i] != null)
            {
                Vector3 pos = rects[i].anchoredPosition;
                pos.y = (2 - i) * itemHeight; // Sprite 2 at center
                rects[i].anchoredPosition = pos;
            }
        }
    }

    public RuntimeSpell GetCurrentSpell()
    {
        if (availableSpells == null || availableSpells.Length == 0) return null;
        return availableSpells[currentIndex];
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    public void SetCurrentIndex(int index)
    {
        if (availableSpells != null && availableSpells.Length > 0)
        {
            currentIndex = index % availableSpells.Length;
            if (currentIndex < 0) currentIndex += availableSpells.Length;
        }
    }
}