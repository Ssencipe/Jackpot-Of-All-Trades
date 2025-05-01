using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Reel : MonoBehaviour
{
    [Header("Spells")]
    public SpellSO[] availableSpells;
    private int currentIndex = 0;

    [Header("Visuals")]
    public Image reelBackground;
    public Image upperSprite;
    public Image lowerSprite;
    public Image lockVisualImage;

    [Header("Spin Settings")]
    public float minSpinDuration = 3f;
    public float maxSpinDuration = 5f;
    public float minSpinSpeed = 0.05f; // Fastest spin speed
    public float maxSpinSpeed = 0.5f;  // Slowest spin speed

    public bool IsLocked { get; private set; } = false;
    private bool isSpinning = false;

    private void Start()
    {
        RandomizeStart();
    }

    public void Spin()
    {
        if (IsLocked || isSpinning) return;

        StartCoroutine(SpinCoroutine());
    }

    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;

        float spinDuration = Random.Range(minSpinDuration, maxSpinDuration);
        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            // Spin faster at the beginning, slower at the end
            float t = elapsed / spinDuration; // 0 at start, 1 at end
            float currentCooldown = Mathf.Lerp(minSpinSpeed, maxSpinSpeed, t);

            // Move the reel (downwards visual)
            currentIndex = (currentIndex - 1 + availableSpells.Length) % availableSpells.Length;
            UpdateVisuals();

            yield return new WaitForSeconds(currentCooldown);
            elapsed += currentCooldown;
        }

        isSpinning = false;
    }

    public void NudgeUp()
    {
        if (IsLocked || isSpinning) return;

        currentIndex = (currentIndex + 1) % availableSpells.Length;
        UpdateVisuals();
    }

    public void NudgeDown()
    {
        if (IsLocked || isSpinning) return;

        currentIndex = (currentIndex - 1 + availableSpells.Length) % availableSpells.Length;
        UpdateVisuals();
    }

    public void Lock()
    {
        IsLocked = true;
        if (lockVisualImage != null)
            lockVisualImage.enabled = true;
    }

    public void Unlock()
    {
        IsLocked = false;
        if (lockVisualImage != null)
            lockVisualImage.enabled = false;
    }

    public void RandomizeStart()
    {
        currentIndex = Random.Range(0, availableSpells.Length);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (reelBackground != null)
            reelBackground.sprite = availableSpells[currentIndex].icon;
        if (upperSprite != null)
            upperSprite.sprite = availableSpells[(currentIndex - 1 + availableSpells.Length) % availableSpells.Length].icon;
        if (lowerSprite != null)
            lowerSprite.sprite = availableSpells[(currentIndex + 1) % availableSpells.Length].icon;
    }

    public SpellSO GetCenterSpell()
    {
        if (availableSpells == null || availableSpells.Length == 0)
            return null;

        return availableSpells[currentIndex];
    }
}
