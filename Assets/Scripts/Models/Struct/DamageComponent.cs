using System.Collections.Generic;

namespace RPG_System
{
    public struct DamageComponent
    {
        public int?       Flat;
        public string     Tag;
        public List<Dice> Dice;

        // Constructeur Dice
        public DamageComponent(int diceNumber, int diceValue, string tag)
        {
            this.Flat = null;
            this.Tag  = tag;
            this.Dice = new List<Dice>();

            for (int i = 0; i < diceNumber; i++)
            {
                this.Dice.Add(new Dice(diceValue, tag));
            }
        }

        // Constructeur Flat
        public DamageComponent(int flat, string tag)
        {
            this.Flat = flat;
            this.Tag  = tag;
            this.Dice = null;
        }

        public (int Result, string Tag) Roll()
        {
            if (this.Flat.HasValue)
                return (this.Flat.Value, this.Tag);

            int total = 0;
            foreach (var die in Dice)
            {
                total += die.Roll.Result;
            }

            return (total, this.Tag);
        }
    }
}
