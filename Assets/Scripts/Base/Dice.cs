using System;

public enum DiceType
{
    // Générique
    Neutre,

    // -- Types de dégâts DnD --
    Bludgeoning,     // contondant
    Piercing,        // perforant
    Slashing,        // tranchant
    Fire,            // feu
    Cold,            // froid
    Acid,            // acide
    Lightning,       // foudre
    Thunder,         // tonnerre
    Poison,          // poison
    Psychic,         // psychique
    Necrotic,        // nécrotique
    Radiant,         // radiant
    Force,           // force pure

    // -- Caractéristiques (Abilities) --
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdom,
    Charisma,

    // -- Compétences (Skills) --
    Acrobatics,
    AnimalHandling,
    Arcana,
    Athletics,
    Deception,
    History,
    Insight,
    Intimidation,
    Investigation,
    Medicine,
    Nature,
    Perception,
    Performance,
    Persuasion,
    Religion,
    SleightOfHand,
    Stealth,
    Survival,

    // -- Autres types utiles --
    Initiative,    // jet d’initiative
    SavingThrow,   // jet de sauvegarde générique
    Attack,        // attaque générique
    Spell,         // pour les sorts génériques
}

public struct Dice
{
    public int Number;
    public int Value;
    public DiceType Type;

    private static Random rng = new Random();

    public Dice(int number
               ,int Value
               ,DiceType type = 0) 
    {
        this.Number = number;
        this.Value = Value;
        this.Type = type;
    }

    private int RollOnce()
    {
        return rng.Next(1, Value + 1);
    }

    public int Roll(int Bonus = 0)
    {
        int total = 0;
        for (int i = 0; i < Number; i++)
        {
            int r = RollOnce();
            total += r;
            RollFX(Bonus, r);
        }
        return total + Bonus;
    }

    private void RollFX(int Bonus = 0, int R = 1)
    {
        DiceType Type = this.Type;
        int DiceNumber = this.Value;
    }
}
