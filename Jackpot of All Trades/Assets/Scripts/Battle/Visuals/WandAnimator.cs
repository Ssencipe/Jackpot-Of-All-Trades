using UnityEngine;
using System.Collections;

public class WandAnimator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationDuration = 0.25f;

    private Quaternion defaultRotation;

    private void Awake()
    {
        defaultRotation = transform.localRotation;
    }

    public IEnumerator RotateTo(float targetZ)
    {
        Quaternion start = transform.localRotation;
        Quaternion end = Quaternion.Euler(0f, 0f, targetZ);

        //easing the rotation
        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            float t = elapsed / rotationDuration;
            float easedT = EaseOutCubic(t); // new easing
            transform.localRotation = Quaternion.Slerp(start, end, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = end;
    }
    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3);
    }

    public IEnumerator RotateToDefault() => RotateTo(defaultRotation.eulerAngles.z);
}
