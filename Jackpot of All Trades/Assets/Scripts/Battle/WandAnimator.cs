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

        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            transform.localRotation = Quaternion.Slerp(start, end, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = end;
    }

    public IEnumerator RotateToDefault()
    {
        yield return RotateTo(defaultRotation.eulerAngles.z);
    }
}
