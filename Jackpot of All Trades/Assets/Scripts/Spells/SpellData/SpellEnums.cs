public enum ColorType   //for pattern system later as well as visuals
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple,
    Orange
}

public enum SpellTag    //tags spell types for tooltips and other systems
{
    Offense,        
    Defense,
    Healing,
    Buff,
    Debuff,
    Tally,              //involves manipulation of an associated value
    Mover,              //involves movement of spells on grid
    Targeting,          //involves manipulation of enemy targeting
    Modifier,           //involves the adjustment of other spells
    Transformer,        //involves spells becoming other spells
    Curse,              //involves a negative effect
    Misc
}

public enum TargetingMode   //for setting up spell specific targeting
{
    Self,
    SingleEnemy,
    AllEnemies,
    SingleAlly,
    AllAllies,
    Player,
    Custom          // Flexible override
}