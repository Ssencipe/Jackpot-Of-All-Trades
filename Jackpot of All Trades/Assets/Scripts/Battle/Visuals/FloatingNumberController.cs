using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class FloatingNumberController : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public float lifetime = 5f;

    private FloatingNumberType numberType;
    private int currentValue;
    private float timeRemaining;

    public event Action<FloatingNumberType> OnDestroyed;

    public void Initialize(int value, FloatingNumberType type)
    {
        numberType = type;
        currentValue = value;
        timeRemaining = lifetime;
        UpdateText();

        // Apply type-based offset
        StartCoroutine(ApplyOffsetNextFrame(GetOffsetForType(type)));
    }

    private Vector2 GetOffsetForType(FloatingNumberType type)
    {
        return type switch
        {
            FloatingNumberType.Heal => new Vector2(-100f, 0f),
            FloatingNumberType.Shield => new Vector2(100f, 0f),
            _ => Vector2.zero
        };
    }

    private IEnumerator ApplyOffsetNextFrame(Vector2 offset)
    {
        yield return null;

        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
            rect.anchoredPosition += offset;
    }

    //merge values of floating numbers of the same type
    public void AddValue(int amount)
    {
        currentValue += amount;
        timeRemaining = lifetime;
        UpdateText();

        StopCoroutine(nameof(PunchEffect)); // Restart if already running
        StartCoroutine(nameof(PunchEffect));
    }


    private void UpdateText()
    {
        if (numberText == null) return;

        numberText.text = currentValue.ToString();

        switch (numberType)
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
    }

    //visual pop when values are merged
    private IEnumerator PunchEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 punchScale = originalScale * 1.3f;

        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float eased = 1f - Mathf.Pow(1f - t, 2); // Ease out

            transform.localScale = Vector3.Lerp(punchScale, originalScale, eased);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private void Update()
    {
        timeRemaining -= Time.deltaTime;
        transform.position += Vector3.up * (Time.deltaTime * 0.5f) * 15f;

        if (timeRemaining <= 0f)
        {
            OnDestroyed?.Invoke(numberType);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(numberType);
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
