namespace DnD.DnD_5e
{
    public struct DamageComponent
    {
        public Dice? Dice;              // null si dégâts fixes
        public int? Flat;               // null si dés
        public DamageType Type;

        public DamageComponent(Dice dice, DamageType type)
        {
            Dice = dice;
            Flat = null;
            Type = type;
        }

        public DamageComponent(int flat, DamageType type)
        {
            Dice = null;
            Flat = flat;
            Type = type;
        }

        public int Roll()
        {
            if (Dice.HasValue)
                return Dice.Value.Roll().total;
            if (Flat.HasValue)
                return Flat.Value;

            return 0;
        }

        public override string ToString()
        {
            if (Dice.HasValue) return $"{Dice.Value} {Type.ToString().ToLower()}";
            if (Flat.HasValue) return $"{Flat.Value} {Type.ToString().ToLower()} (fixe)";
            return $"0 {Type.ToString().ToLower()}";
        }
    }

}
