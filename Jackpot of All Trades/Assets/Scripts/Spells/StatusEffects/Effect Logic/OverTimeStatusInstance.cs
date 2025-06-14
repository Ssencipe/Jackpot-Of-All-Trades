using UnityEngine;

public enum OverTimeType { Damage, Heal, Shield }
public enum TickTiming { StartOfTurn, EndOfTurn }

public class OverTimeStatusInstance : IStatusEffect
{
    public string ID => $"OverTime_{type}_{potency}";

    private int turnsLeft;
    public int Duration => turnsLeft;
    public Sprite Icon { get; }

    private readonly int potency;
    private readonly OverTimeType type;
    private readonly TickTiming tickTiming;

    public OverTimeStatusInstance(int potency, int duration, OverTimeType type, TickTiming timing, Sprite icon)
    {
        this.potency = potency;
        this.turnsLeft = duration;
        this.type = type;
        this.tickTiming = timing;
        this.Icon = icon;
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
    }
}
