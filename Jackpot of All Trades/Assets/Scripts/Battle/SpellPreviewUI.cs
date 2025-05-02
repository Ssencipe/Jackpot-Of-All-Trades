using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellPreviewUI : MonoBehaviour
{
    public GameObject spellIconPrefab; // Prefab with Image component
    public Transform iconParent;       // Layout group container
    public Image backgroundPanel;      // Optional background image

    private List<GameObject> activeIcons = new List<GameObject>();

    public void Display(List<BaseSpell> spells)
    {
        Clear();

        if (backgroundPanel != null)
            backgroundPanel.enabled = true;

        foreach (var spell in spells)
        {
            GameObject icon = Instantiate(spellIconPrefab, iconParent);
            icon.GetComponent<Image>().sprite = spell.spellData.icon;
            activeIcons.Add(icon);
        }
    }

    public void Clear()
    {
        foreach (var icon in activeIcons)
        {
            Destroy(icon);
        }
        activeIcons.Clear();

        if (backgroundPanel != null)
            backgroundPanel.enabled = false;
    }
}
