using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyReelVisual : MonoBehaviour
{
    [Header("Visual Components")]
    public GameObject upperSpriteObject;
    public GameObject middleSpriteObject;
    public GameObject lowerSpriteObject;
    
    [Header("Animation Settings")]
    public float itemHeight = 100f;
    public AnimationCurve spinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Image upperSprite;
    private Image middleSprite;
    private Image lowerSprite;
    private RectTransform upperRect;
    private RectTransform middleRect;
    private RectTransform lowerRect;
    private RuntimeSpell[] availableSpells;
    private int currentIndex = 0;
    
    void Awake()
    {
        // Get Image components and their RectTransforms
        if (upperSpriteObject != null)
        {
            upperSprite = upperSpriteObject.GetComponent<Image>();
            upperRect = upperSpriteObject.GetComponent<RectTransform>();
        }
        if (middleSpriteObject != null)
        {
            middleSprite = middleSpriteObject.GetComponent<Image>();
            middleRect = middleSpriteObject.GetComponent<RectTransform>();
        }
        if (lowerSpriteObject != null)
        {
            lowerSprite = lowerSpriteObject.GetComponent<Image>();
            lowerRect = lowerSpriteObject.GetComponent<RectTransform>();
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
        
        // Calculate total distance to travel (multiple rotations for visual effect)
        int extraRotations = Random.Range(3, 6);
        int finalIndex = Random.Range(0, spells.Length);
        float totalDistance = (extraRotations * spells.Length + finalIndex) * itemHeight;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // Apply easing curve
            float easedProgress = spinCurve.Evaluate(progress);
            
            // Calculate current position
            float currentPosition = startPosition + (totalDistance * easedProgress);
            
            // Update visual position
            UpdateScrollPosition(currentPosition);
            
            yield return null;
        }
        
        // Ensure final position is exact
        currentIndex = finalIndex;
        UpdateStaticVisuals(currentIndex);
        
        // Reset position
        ResetPosition();
    }
    
    private void UpdateScrollPosition(float scrollPosition)
    {
        if (availableSpells == null || availableSpells.Length == 0) return;
        
        // Calculate which spells should be visible
        float normalizedPosition = scrollPosition / itemHeight;
        int baseIndex = Mathf.FloorToInt(normalizedPosition) % availableSpells.Length;
        
        // Handle negative indices
        while (baseIndex < 0) baseIndex += availableSpells.Length;
        
        // Update the three visible sprites
        int upperIndex = (baseIndex - 1 + availableSpells.Length) % availableSpells.Length;
        int middleIndex = baseIndex;
        int lowerIndex = (baseIndex + 1) % availableSpells.Length;
        
        UpdateSpriteImage(upperSprite, availableSpells[upperIndex]);
        UpdateSpriteImage(middleSprite, availableSpells[middleIndex]);
        UpdateSpriteImage(lowerSprite, availableSpells[lowerIndex]);
        
        // Move individual sprites instead of the container
        float positionOffset = -(scrollPosition % itemHeight);
        
        if (upperRect != null)
        {
            Vector3 pos = upperRect.anchoredPosition;
            pos.y = positionOffset + itemHeight; // Upper sprite offset
            upperRect.anchoredPosition = pos;
        }
        
        if (middleRect != null)
        {
            Vector3 pos = middleRect.anchoredPosition;
            pos.y = positionOffset; // Middle sprite at center
            middleRect.anchoredPosition = pos;
        }
        
        if (lowerRect != null)
        {
            Vector3 pos = lowerRect.anchoredPosition;
            pos.y = positionOffset - itemHeight; // Lower sprite offset
            lowerRect.anchoredPosition = pos;
        }
    }
    
    private void UpdateStaticVisuals(int centerIndex)
    {
        if (availableSpells == null || availableSpells.Length == 0) return;
        
        int upperIndex = (centerIndex - 1 + availableSpells.Length) % availableSpells.Length;
        int lowerIndex = (centerIndex + 1) % availableSpells.Length;
        
        UpdateSpriteImage(upperSprite, availableSpells[upperIndex]);
        UpdateSpriteImage(middleSprite, availableSpells[centerIndex]);
        UpdateSpriteImage(lowerSprite, availableSpells[lowerIndex]);
        
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
        // Reset individual sprite positions instead of container
        if (upperRect != null)
        {
            Vector3 pos = upperRect.anchoredPosition;
            pos.y = itemHeight;
            upperRect.anchoredPosition = pos;
        }
        
        if (middleRect != null)
        {
            Vector3 pos = middleRect.anchoredPosition;
            pos.y = 0f;
            middleRect.anchoredPosition = pos;
        }
        
        if (lowerRect != null)
        {
            Vector3 pos = lowerRect.anchoredPosition;
            pos.y = -itemHeight;
            lowerRect.anchoredPosition = pos;
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
}