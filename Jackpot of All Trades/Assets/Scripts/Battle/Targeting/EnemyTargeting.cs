public enum EnemyTargeting
{
    Self,           // Always self-targets
    AllyOnly,       // Random other ally (excludes self)
    WeakestAlly,    // Lowest current health
    StrongestAlly,  // Highest impactScore
    Random          // Any alive ally (including self)
}