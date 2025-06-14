using UnityEngine;

public interface IStatusEffect
{
    string ID { get; } // Used to avoid duplicates
    Sprite Icon { get; }
    int Duration { get; }

    void OnApply(ITargetable target);
    void OnTurnStart(ITargetable target);
    void OnTurnEnd(ITargetable target);
    void OnExpire(ITargetable target);
}