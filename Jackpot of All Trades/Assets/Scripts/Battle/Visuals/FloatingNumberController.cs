using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingNumberController : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public float moveSpeed = 0.000001f;
    public float lifetime = 5f;

    private float alpha = 1f;

    //determine type for color
    public void Initialize(int value, FloatingNumberType type)
    {
        Debug.Log($"[FloatingNumber] Initialize called with value: {value}");


        if (numberText == null) return;

        numberText.text = value.ToString();

        switch (type)
        {
            case FloatingNumberType.Damage:
                numberText.color = Color.red;
                break;
            case FloatingNumberType.Heal:
                numberText.color = Color.green;
                break;
            case FloatingNumberType.Shield:
                numberText.color = Color.blue;
                break;
        }

        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        Debug.Log($"[FloatingNumber] Starting with value: {numberText.text}");


        float elapsedTime = 0f;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * 1f;

        Vector3 minScale = Vector3.one * 0.5f;
        Vector3 maxScale = Vector3.one;

        while (elapsedTime < lifetime)
        {
            float t = elapsedTime / lifetime;
            float easedT = EaseOutQuad(t);

            // movement
            transform.position = Vector3.Lerp(start, end, easedT);

            // scale bounce (only during early portion)
            float scaleT = Mathf.Clamp01(t * 4f); // scale easing in first ~25% of lifetime
            float bounce = EaseOutBack(scaleT);
            transform.localScale = Vector3.LerpUnclamped(minScale, maxScale, bounce);

            // fade out
            alpha = Mathf.Lerp(1f, 0f, t);
            if (numberText != null)
            {
                var color = numberText.color;
                color.a = alpha;
                numberText.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    //for speed
    private float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    //for scale
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

}

//the types for color
[System.Serializable]
public struct FloatingNumberData
{
    public int value;
    public FloatingNumberType type;

    public FloatingNumberData(int value, FloatingNumberType type)
    {
        this.value = value;
        this.type = type;
    }
}

public enum FloatingNumberType
{
    Damage,
    Heal,
    Shield
}
