using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellPreviewUI : MonoBehaviour
{
    public GameObject spellIconPrefab; // Prefab with Image component
    public Transform iconParent;       // Layout group container
    public Image backgroundPanel;      // Optional background image

    //the image being instantiated
    private List<GameObject> activeIcons = new List<GameObject>();

    //the spells being cast
    private List<BaseSpell> activeSpells = new List<BaseSpell>();

    //initialize
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
        {
            Destroy(icon);
        }
        activeIcons.Clear();

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

            if (spell.runtimeSpell != null && spell.runtimeSpell.isDisabled)
            {
                // Fade the icon visually to indicate it is disabled
                img.color = new Color(0.7f, 0.7f, 0.7f, 0.4f);
            }

            var group = iconGO.GetComponent<CanvasGroup>();
            if (group != null)
                group.alpha = 0f;

            iconGO.transform.localScale = Vector3.one;

            activeIcons.Add(iconGO);
            activeSpells.Add(spell);
            iconObjects.Add(iconGO);
        }

        // Wait for layout to process
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


    //visual scale pop effect stuff below for indicating which spell is being cast
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
