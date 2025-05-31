namespace DnD.DnD_5e
{
    public enum DamageType
    {
        Contondant, Perforant, Tranchant, Force,
        Feu, Froid, Foudre, Tonnerre,
        Acide, Poison, Radiant, Nécrotique, Psychique
    }

    public struct DamageDice
    {
        public Dice Dice;
        public DamageType Type;

        public DamageDice(Dice dice, DamageType type)
        {
            Dice = dice;
            Type = type;
        }
    }
}
