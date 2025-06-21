namespace DnD.DnD_5e
{
    [System.Flags]
    public enum ActivContext
    {
        NoFight     = 1 << 0,
        Action      = 1 << 1,
        BonusAction = 1 << 2,
        Reaction    = 1 << 3
    }
}