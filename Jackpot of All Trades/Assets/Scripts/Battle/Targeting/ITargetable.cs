public interface ITargetable
{
    void TakeDamage(int amount);
    void Heal(int amount);
    void GainShield(int amount);
    void ResetShield();
    int currentHP { get; }
}