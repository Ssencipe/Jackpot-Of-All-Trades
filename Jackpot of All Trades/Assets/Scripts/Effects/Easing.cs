using UnityEngine;

//reusable easing functions for visuals
public static class Easing
{
    public static float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }

    public static float EaseOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t);
    }

    //it goes up and it goes down
    public static float EaseOutBounce(float t)
    {
        if (t < 1 / 2.75f)
        {
            return 7.5625f * t * t;
        }
        else if (t < 2 / 2.75f)
        {
            t -= 1.5f / 2.75f;
            return 7.5625f * t * t + 0.75f;
        }
        else if (t < 2.5 / 2.75f)
        {
            t -= 2.25f / 2.75f;
            return 7.5625f * t * t + 0.9375f;
        }
        else
        {
            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        }
    }

    //ported from SpellPreviewUI
    public static float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    //returns a scale factor that oscillates then settles at 1.0.
    public static float Wobble(float t, float amplitude = 0.25f, float frequency = 3f, float damping = 4f)
    {
        // Clamp t between 0 and 1 for stability
        t = Mathf.Clamp01(t);
        return 1 + amplitude * Mathf.Exp(-damping * t) * Mathf.Cos(frequency * t * 2 * Mathf.PI);
    }

}