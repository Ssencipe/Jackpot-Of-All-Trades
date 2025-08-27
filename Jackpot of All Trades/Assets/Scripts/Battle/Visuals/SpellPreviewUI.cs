using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellPreviewUI : MonoBehaviour
{
    [Header("Prefab & Layout")]
    public GameObject spellIconPrefab;        // Prefab with Image component and CanvasGroup
    public GameObject modificationLabelPrefab; // Prefab with TextMeshProUGUI + CanvasGroup
    public Transform iconParent;              // Layout group container
    public Image backgroundPanel;             // Optional background image

    private List<GameObject> activeIcons = new();   // Instantiated spell icons
    private List<GameObject> activeLabels = new();  // Instantiated BUFF/NERF/SKIP labels
    private List<BaseSpell> activeSpells = new();   // Spells being cast

    // initialize UI from spell list
    public void Display(List<BaseSpell> spells)
    {
        Clear();

        if (backgroundPanel != null)
            backgroundPanel.enabled = true;

        StartCoroutine(FadeInSequence(spells));
    }

    public void Clear()
    {
        foreach (var icon in activeIcons)
            Destroy(icon);
        activeIcons.Clear();

        foreach (var label in activeLabels)
            Destroy(label);
        activeLabels.Clear();

        if (backgroundPanel != null)
            backgroundPanel.enabled = false;
    }

    private IEnumerator FadeInSequence(List<BaseSpell> spells)
    {
        float staggerDelay = 0.1f;
        activeSpells.Clear();

        var layoutGroup = iconParent.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
            layoutGroup.enabled = false;

        List<GameObject> iconObjects = new();

        foreach (var spell in spells)
        {
            GameObject iconGO = Instantiate(spellIconPrefab, iconParent);

            var img = iconGO.GetComponent<Image>();
            img.sprite = spell.spellData.icon;

            // Set color based on modification
            img.color = SpellVisualUtil.GetColorForRuntimeSpell(spell.runtimeSpell);

            var group = iconGO.GetComponent<CanvasGroup>();
            if (group != null)
                group.alpha = 0f;

            iconGO.transform.localScale = Vector3.one;

            // Add label if spell is modified
            string labelText = GetModificationLabel(spell.runtimeSpell);
            if (!string.IsNullOrEmpty(labelText) && modificationLabelPrefab != null)
            {
                GameObject labelGO = Instantiate(modificationLabelPrefab, iconGO.transform);
                labelGO.transform.localPosition = new Vector3(0f, 1f, 0f); // Adjust label position under icon

                var text = labelGO.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = labelText;

                    // Set color based on label
                    switch (labelText)
                    {
                        case "BUFF": text.color = Color.cyan; break;
                        case "NERF": text.color = new Color(1f, 0.65f, 0f); break;
                        case "SKIP": text.color = Color.gray; break;
                    }
                }

                activeLabels.Add(labelGO);
            }

            activeIcons.Add(iconGO);
            activeSpells.Add(spell);
            iconObjects.Add(iconGO);
        }

        // Wait for layout rebuild
        yield return new WaitForEndOfFrame();

        if (layoutGroup != null)
        {
            layoutGroup.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(iconParent as RectTransform);
        }

        foreach (var iconGO in iconObjects)
        {
            var group = iconGO.GetComponent<CanvasGroup>();
            StartCoroutine(FadeInCanvasGroup(group));
            yield return new WaitForSeconds(staggerDelay);
        }
    }

    private string GetModificationLabel(RuntimeSpell spell)
    {
        if (spell == null) return null;

        if (spell.wasMarkedToSkip)
            return "SKIP";
        if (spell.wasPotencyModified)
            return spell.potencyMultiplier < 1f ? "NERF" : "BUFF";

        return null;
    }

    // canvas fade for preview icons
    private IEnumerator FadeInCanvasGroup(CanvasGroup group)
    {
        if (group == null) yield break;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            group.alpha = Easing.EaseOutCubic(t);
            yield return null;
        }

        group.alpha = 1f;
    }

    // highlight animation when spell casts
    public void PlayPopEffect(int index)
    {
        if (index < 0 || index >= activeIcons.Count) return;

        Transform icon = activeIcons[index].transform;
        StartCoroutine(PlayScalePop(icon));
    }

    private IEnumerator PlayScalePop(Transform iconTransform)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float scale = Easing.Wobble(t, 0.4f, 4f, 5f); // tweak amplitude, frequency, damping

            iconTransform.localScale = Vector3.one * scale;

            elapsed += Time.deltaTime;
            yield return null;
        }

        iconTransform.localScale = Vector3.one;
    }
}