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
    private RuntimeSpell[] availableSpells;
    private int currentIndex = 0;
    private RectTransform rectTransform;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // Get Image components from GameObjects
        if (upperSpriteObject != null)
            upperSprite = upperSpriteObject.GetComponent<Image>();
        if (middleSpriteObject != null)
            middleSprite = middleSpriteObject.GetComponent<Image>();
        if (lowerSpriteObject != null)
            lowerSprite = lowerSpriteObject.GetComponent<Image>();
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
        if (rectTransform != null)
        {
            Vector3 pos = rectTransform.anchoredPosition;
            pos.y = 0f;
            rectTransform.anchoredPosition = pos;
        }
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
        
        // Update visual position offset
        float positionOffset = -(scrollPosition % itemHeight);
        if (rectTransform != null)
        {
            Vector3 pos = rectTransform.anchoredPosition;
            pos.y = positionOffset;
            rectTransform.anchoredPosition = pos;
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