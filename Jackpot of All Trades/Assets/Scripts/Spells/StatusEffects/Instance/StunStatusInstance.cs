using UnityEngine;

public class StunStatusInstance : IStatusEffect
{
    public string ID => "Stun";

    private int turnsLeft;
    public int Duration => turnsLeft;
    public Sprite Icon { get; }
    public string Label { get; }
    public string SourceSpellName { get; }
    public Sprite SourceIcon { get; }
    private readonly string effectSound;

    public StunStatusInstance(int duration, Sprite icon = null, string label = null, string sourceSpellName = null, Sprite sourceIcon = null, string effectSound = null)
    {
        this.turnsLeft = Mathf.Max(1, duration);
        this.Icon = icon;
        this.Label = label ?? "Stunned";
        this.SourceSpellName = sourceSpellName;
        this.SourceIcon = sourceIcon;
        this.effectSound = effectSound;
    }

    public void OnApply(ITargetable target)
    {
        if (!string.IsNullOrEmpty(effectSound))
        {
            Debug.Log($"[OverTimeStatusInstance] Playing tick sound: {effectSound}");
            AudioManager.Instance.PlaySFX(effectSound, AudioManager.Instance.spellLibrary);
        }

        Debug.Log($"[StunStatus] Applied to {target} for {turnsLeft} turn(s).");
    }

    public void OnTurnStart(ITargetable target)
    {
        // Stun simply prevents actions; duration is decremented at end of the target's turn in this implementation.
    }

    public void OnTurnEnd(ITargetable target)
    {
        turnsLeft--;
        if (turnsLeft <= 0)
            OnExpire(target);
    }

    public void OnExpire(ITargetable target)
    {
        Debug.Log($"[StunStatus] Expired on {target}.");
        // Cleanup visuals/sounds if needed
    }

    public void Refresh(IStatusEffect newEffect)
    {
        if (newEffect is StunStatusInstance incoming)
        {
            // Simple refresh: add remaining durations (alternative: reset to max)
            this.turnsLeft += incoming.turnsLeft;
        }
    }

    public string GetTooltip()
    {
        return $"{Label} — skips actions for {turnsLeft} turn(s).";
    }
}