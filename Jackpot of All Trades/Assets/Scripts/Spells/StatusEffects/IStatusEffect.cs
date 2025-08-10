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

    //Called if existing effect is called again to stack duration
    void Refresh(IStatusEffect newInstance);
}