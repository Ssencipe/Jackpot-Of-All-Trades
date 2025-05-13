using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellPreviewUI : MonoBehaviour
{
    public GameObject spellIconPrefab; // Prefab with Image component
    public Transform iconParent;       // Layout group container
    public Image backgroundPanel;      // Optional background image

    private List<GameObject> activeIcons = new List<GameObject>();

    //initialize
    public void Display(List<BaseSpell> spells)
    {
        Clear();

        if (backgroundPanel != null)
            backgroundPanel.enabled = true;

        StartCoroutine(FadeInSequence(spells));
    }

    //remove
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

    //animate in and out
    private IEnumerator AnimateIn(CanvasGroup group, Transform iconTransform)
    {
        float duration = 0.25f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.6f;
        Vector3 endScale = Vector3.one;

        iconTransform.localScale = startScale;
        group.alpha = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            iconTransform.localScale = Vector3.Lerp(startScale, endScale, EaseOutBack(t));
            group.alpha = Mathf.Lerp(0f, 1f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        iconTransform.localScale = endScale;
        group.alpha = 1;
    }

    //easing animation
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    private IEnumerator FadeInSequence(List<BaseSpell> spells)
    {
        float fadeDuration = 0.3f;
        float staggerDelay = 0.1f;

        List<Image> imagesToFade = new List<Image>();

        // Pre-instantiate all icons and hide them
        foreach (var spell in spells)
        {
            GameObject iconGO = Instantiate(spellIconPrefab, iconParent);
            Image img = iconGO.GetComponent<Image>();
            img.sprite = spell.spellData.icon;

            // Set alpha 0 and smaller scale for pop-in
            Color c = img.color;
            c.a = 0f;
            img.color = c;
            img.transform.localScale = Vector3.one * 0.8f;

            activeIcons.Add(iconGO);
            imagesToFade.Add(img);
        }

        // fade in sequentially
        foreach (var img in imagesToFade)
        {
            StartCoroutine(FadeInImage(img, fadeDuration));
            yield return new WaitForSeconds(staggerDelay);
        }
    }

    private IEnumerator FadeInImage(Image img, float duration)
    {
        float elapsed = 0f;
        Color c = img.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            c.a = Mathf.SmoothStep(0f, 1f, t);  // Easing in
            img.color = c;
            yield return null;
        }

        c.a = 1f;
        img.color = c;
    }
}
