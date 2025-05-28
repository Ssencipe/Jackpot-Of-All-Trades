using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EnemyReelUI : MonoBehaviour
{
    public EnemyReel linkedReel;
    public Image centerSpellImage, upperSpellImage, lowerSpellImage;

    private void Start()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (linkedReel == null || linkedReel.availableSpells == null || linkedReel.availableSpells.Length == 0)
            return;

        int currentIndex = linkedReel.GetCurrentIndex();
        SpellSO[] spells = linkedReel.availableSpells;

        if (centerSpellImage != null)
            centerSpellImage.sprite = spells[currentIndex].icon;
        if (upperSpellImage != null)
            upperSpellImage.sprite = spells[(currentIndex - 1 + spells.Length) % spells.Length].icon;
        if (lowerSpellImage != null)
            lowerSpellImage.sprite = spells[(currentIndex + 1) % spells.Length].icon;
    }
}
