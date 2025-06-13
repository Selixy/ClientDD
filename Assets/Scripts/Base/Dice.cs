using System;
using System.Collections.Generic;

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

    public (int total, List<(int value, bool kept)> rolls) Roll(int bonus = 0, int isAdvantage = 0)
    {
        int total1 = 0;
        List<int> rolls1 = new();
        for (int i = 0; i < Number; i++)
        {
            int r = RollOnce();
            total1 += r;
            rolls1.Add(r);
            RollFX(bonus, r);
        }

        // Pas d'avantage/désavantage → retour direct
        if (isAdvantage == 0)
        {
            List<(int, bool)> finalRolls = new();
            foreach (var r in rolls1)
                finalRolls.Add((r, true));

            return (total1 + bonus, finalRolls);
        }

        // Deuxième série de jets pour comparaison
        int total2 = 0;
        List<int> rolls2 = new();
        for (int i = 0; i < Number; i++)
        {
            int r = RollOnce();
            total2 += r;
            rolls2.Add(r);
            RollFX(bonus, r);
        }

        // Choix du meilleur ou du pire selon avantage/désavantage
        bool useSecond = (isAdvantage < 0 && total2 < total1) || (isAdvantage > 0 && total2 > total1);
        var usedRolls = useSecond ? rolls2 : rolls1;
        var discardedRolls = useSecond ? rolls1 : rolls2;
        int total = useSecond ? total2 : total1;

        List<(int, bool)> allRolls = new();
        foreach (var r in usedRolls)
            allRolls.Add((r, true));
        foreach (var r in discardedRolls)
            allRolls.Add((r, false));

        return (total + bonus, allRolls);
    }



    private void RollFX(int Bonus = 0, int R = 1)
    {
        DiceType Type = this.Type;
        int DiceNumber = this.Value;
    }
}
