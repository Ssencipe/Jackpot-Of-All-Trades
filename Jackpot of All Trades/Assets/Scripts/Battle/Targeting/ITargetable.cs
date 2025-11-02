public interface ITargetable
{
    void TakeDamage(int amount);
    void Heal(int amount);
    void GainShield(int amount);
    void ResetShield();
    int currentHP { get; }
}

// Different targenting results and reference data. May want to merge into another script.