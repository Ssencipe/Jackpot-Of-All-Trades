using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class EnemyReel : MonoBehaviour
{
    [Header("Spells")]
    public RuntimeSpell[] availableSpells;
    private int currentIndex = 0;

    [Header("Visuals")]
    public Image reelBackground;
    public Image upperSprite;
    public Image lowerSprite;
    
    // Add EnemyReelVisual reference
    [Header("Dependencies")]
    public EnemyReelVisual enemyReelVisual;

    [Header("Spin Settings")]
    public float minSpinDuration = 3f;
    public float maxSpinDuration = 6f;
    public float minSpinSpeed = 100f;
    public float maxSpinSpeed = 800f;

    private bool isSpinning = false;

    public EnemyReelUI linkedUI;

    private void Start()
    {
        RandomizeStart();
    }

    // Spins the reel for a random duration and updates visuals.
    public void Spin()
    {
        if (isSpinning || availableSpells == null || availableSpells.Length == 0) return;
        StartCoroutine(SpinCoroutine());
    }

    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;

        float spinDuration = UnityEngine.Random.Range(minSpinDuration, maxSpinDuration);
        
        // Use EnemyReelVisual for smooth animation
        if (enemyReelVisual != null)
        {
            yield return StartCoroutine(enemyReelVisual.ScrollSpells(spinDuration, minSpinSpeed, maxSpinSpeed, availableSpells));
            
            // Update current index from visual
            var currentSpell = enemyReelVisual.GetCurrentSpell();
            if (currentSpell != null)
            {
                for (int i = 0; i < availableSpells.Length; i++)
                {
                    if (availableSpells[i] == currentSpell)
                    {
                        currentIndex = i;
                        break;
                    }
                }
            }
        }
        else
        {
            // Fallback to old system if EnemyReelVisual not assigned
            yield return StartCoroutine(OldSpinAnimation(spinDuration));
        }

        isSpinning = false;

        // Final sync
        if (linkedUI == null)
            linkedUI = GetComponentInChildren<EnemyReelUI>();
        linkedUI?.UpdateVisuals();
    }

    // Randomizes the start position of the reel.
    public void RandomizeStart()
    {
        if (availableSpells == null || availableSpells.Length == 0) return;
        
        currentIndex = UnityEngine.Random.Range(0, availableSpells.Length);
        
        // Use EnemyReelVisual for initialization
        if (enemyReelVisual != null)
        {
            enemyReelVisual.InitializeVisuals(availableSpells);
        }
        else
        {
            UpdateVisuals();
        }
    }

    // Keep old animation as fallback
    private IEnumerator OldSpinAnimation(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT = 1 - Mathf.Pow(1 - t, 3); // EaseOutCubic
            float currentCooldown = Mathf.Lerp(minSpinSpeed, maxSpinSpeed, easedT);

            currentIndex = (currentIndex - 1 + availableSpells.Length) % availableSpells.Length;
            UpdateVisuals();

            yield return new WaitForSeconds(currentCooldown);
            elapsed += currentCooldown;
        }
    }

    // Updates the reel's visuals to reflect the current state.
    private void UpdateVisuals()
    {
        if (availableSpells == null || availableSpells.Length == 0) return;

        if (reelBackground != null)
            reelBackground.sprite = availableSpells[currentIndex].icon;

        if (upperSprite != null)
        {
            upperSprite.sprite = availableSpells[(currentIndex - 1 + availableSpells.Length) % availableSpells.Length].icon;

            //tilted visuals
            upperSprite.transform.localRotation = Quaternion.Euler(1f, 0f, 0f);
        }

        if (lowerSprite != null)
        {
            lowerSprite.sprite = availableSpells[(currentIndex + 1) % availableSpells.Length].icon;

            //tilted visuals
            lowerSprite.transform.localRotation = Quaternion.Euler(-1f, 0f, 0f);
        }
    }

    // Returns the spell in the center of the reel (for intent).
    public RuntimeSpell GetCenterSpell()
    {
        if (availableSpells == null || availableSpells.Length == 0) return null;
        return availableSpells[currentIndex];
    }

    // Returns the index of the center spell (for UI).
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    // Returns true if the reel is currently spinning.
    public bool IsSpinning()
    {
        return isSpinning;
    }
}
