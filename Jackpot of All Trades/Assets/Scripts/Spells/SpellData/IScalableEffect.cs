public interface IScalableEffect : ISpellEffect
{
    void SetScaleMultiplier(int multiplier);
}

// For scaling a spell effect based on another factor (a multiplier to spell effects)