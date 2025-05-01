using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingNumberController : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public float moveSpeed = 1f;
    public float lifetime = 3f;

    private float alpha = 1f;

    public void Initialize(int value, FloatingNumberType type)
    {
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
        float elapsedTime = 0f;

        while (elapsedTime < lifetime)
        {
            elapsedTime += Time.deltaTime;

            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            alpha = Mathf.Lerp(1f, 0f, elapsedTime / lifetime);
            if (numberText != null)
            {
                var color = numberText.color;
                color.a = alpha;
                numberText.color = color;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}

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
