using UnityEngine;
using System.Collections;

public class ScreenShakeController : MonoBehaviour
{
    private Vector3 originalLocalPos;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        originalLocalPos = transform.localPosition;
    }

    public void Shake(float duration = 0.2f, float magnitude = 0.2f)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            Vector2 offset = Random.insideUnitCircle * magnitude;
            transform.localPosition = originalLocalPos + new Vector3(offset.x, offset.y, 0f);

            yield return null;
        }

        transform.localPosition = originalLocalPos;
    }
}