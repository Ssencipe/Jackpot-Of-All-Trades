using UnityEngine;

public enum OverTimeType { Damage, Heal, Shield }
public enum TickTiming { StartOfTurn, EndOfTurn }

public class OverTimeStatusInstance : IStatusEffect
{
    public string ID => $"OverTime_{type}";

    private int turnsLeft;
    public int Duration => turnsLeft;
    public Sprite Icon { get; }

    private readonly int potency;
    private readonly OverTimeType type;
    private readonly TickTiming tickTiming;

    //reference versions of above variables for other scripts
    public int Potency => potency;
    public OverTimeType Type => type;
    public TickTiming TickTiming => tickTiming;
    private readonly string effectSound;


    public OverTimeStatusInstance(int potency, int duration, OverTimeType type, TickTiming timing, Sprite icon, string effectSound = null)
    {
        this.potency = potency;
        this.turnsLeft = duration;
        this.type = type;
        this.tickTiming = timing;
        this.Icon = icon;
        this.effectSound = effectSound;
    }

    public void OnApply(ITargetable target)
    {
        Debug.Log($"Applied {type} OverTime for {turnsLeft} turns.");
    }

    public void OnTurnStart(ITargetable target)
    {
        if (tickTiming != TickTiming.StartOfTurn) return;
        ApplyEffect(target);
    }

    public void OnTurnEnd(ITargetable target)
    {
        if (tickTiming == TickTiming.EndOfTurn)
            ApplyEffect(target);

        turnsLeft--;
        if (turnsLeft <= 0)
            OnExpire(target);
    }

    public void OnExpire(ITargetable target)
    {
        Debug.Log($"Status {ID} expired.");
    }

    //Handle refreshing or stacking duration
    public void Refresh(IStatusEffect newEffect)
    {
        if (newEffect is OverTimeStatusInstance incoming)
        {
            // OPTION 1: Stack duration
            turnsLeft += incoming.turnsLeft;

            // OPTION 2: Or refresh to max
            // turnsLeft = Mathf.Max(turnsLeft, incoming.turnsLeft);
        }
    }

    private void ApplyEffect(ITargetable target)
    {
        switch (type)
        {
            case OverTimeType.Damage:
                target.TakeDamage(potency);
                break;
            case OverTimeType.Heal:
                target.Heal(potency);
                break;
            case OverTimeType.Shield:
                target.GainShield(potency);
                break;
        }

        if (!string.IsNullOrEmpty(effectSound))
        {
            Debug.Log($"[OverTimeStatusInstance] Playing tick sound: {effectSound}");
            AudioManager.Instance.PlaySFX(effectSound, AudioManager.Instance.spellLibrary);
        }
    }

    //Tooltip text
    public string GetTooltip()
    {
        return $"{type} {potency} per turn ({turnsLeft} left)";
    }
}
