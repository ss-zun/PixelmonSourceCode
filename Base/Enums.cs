
#region ∞≥¿Œ
public enum AbilityType
{
    Attack,
    Trait,
    PSVAtk,
    PSVCri,
    PSVCriDmg,
    PSVAtkSpd,
    PSVDmg,
    PSVSDmg,
    PSVSCri,
    PSVSCriDmg,
    PlayerHP,
    PlayerDefense
}

public enum PxmRank
{
    Common,
    Advanced,
    Rare,
    Epic,
    Legendary,
    Unique
}

public enum PsvRank
{
    F,
    E,
    D,
    C,
    B,
    A,
    S,
    SS
}
#endregion

public enum PixelmonState
{
    Idle,
    Move,
    Attack
}

public enum FieldState
{
    Locked,
    Buyable,
    Empty,
    Seeded,
    Harvest
}

public enum TraitType
{
    AddDmg,
    AddCriDmg,
    AddSDmg,
    AddSCriDmg
}

public enum PassiveType
{
    Attack,
    Buff,
    Farming
}

public enum DirtyUI
{
    Gold,
    Diamond,
    Seed,
    Food,
    SkillTicket,
    UserExp,
    EggCount,
    EggLv
}

public enum TabState
{
    Normal,
    Equip,
    UnEquip,
    Empty
}

public enum AtvSkillType
{
    Single,
    Area,
    RandomSpot,
    Special
}