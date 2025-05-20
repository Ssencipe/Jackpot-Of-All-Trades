using UnityEngine;
using System.Collections;

public enum FlashType
{
    Damage,
    Heal,
    Shield
}

public class FlashEffect : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Flash Colors")]
    public Color damageColor = new Color(1f, 0.8f, 0.8f); // Light pink
    public Color healColor = new Color(0.6f, 1f, 0.6f);   // Light green
    public Color shieldColor = new Color(0.6f, 0.8f, 1f); // Light blue

    public float flashDuration = 1f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private Coroutine flashRoutine;

    public void Flash(FlashType type)
    {
        if (sr == null) return;

        // Choose color
        Color flashColor = type switch
        {
            FlashType.Heal => healColor,
            FlashType.Shield => shieldColor,
            _ => damageColor
        };

        // Cancel previous flash
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(DoFlash(flashColor));
    }

    private IEnumerator DoFlash(Color flashColor)
    {
        Color originalColor = sr.color;
        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }
}
