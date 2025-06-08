using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public struct State
    {
        public int Str { get; set; }
        public int Dex { get; set; }
        public int Con { get; set; }
        public int Int { get; set; }
        public int Wis { get; set; }
        public int Cha { get; set; }

        public State(int str, int dex, int con, int intel, int wis, int cha)
        {
            Str = str;
            Dex = dex;
            Con = con;
            Int = intel;
            Wis = wis;
            Cha = cha;
        }
        
        public State mod
        {
            get
            {            
                int mStr  = (int)System.Math.Floor((Str - 10) / 2.0);
                int mDex  = (int)System.Math.Floor((Dex - 10) / 2.0);
                int mCon  = (int)System.Math.Floor((Con - 10) / 2.0);
                int mInt  = (int)System.Math.Floor((Int - 10) / 2.0);
                int mWis  = (int)System.Math.Floor((Wis - 10) / 2.0);
                int mCha  = (int)System.Math.Floor((Cha - 10) / 2.0);

                return new State(mStr, mDex, mCon, mInt, mWis, mCha);
            }
        }
    }
    
    public enum DnDRollType
    {
        // Jets de sauvegarde (Saving Throws)
        StrengthSave,
        DexteritySave,
        ConstitutionSave,
        IntelligenceSave,
        WisdomSave,
        CharismaSave,

        // Jet de caractéristique
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma,

        // Jets de compétence (Skill Checks)
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
    }

    public class Entity_DnD_5e : Entity<Item_DnD_5e, Etat_DnD_5e>
    {
        public string            Race           { get; protected set; }
        public string            Class          { get; protected set; }
        public State             Stats          { get; protected set; }
        public State             Modifiers      { get; protected set; }

        public ActivContext      fightContext   { get; set; }
        public List<State>           Debuffs    { get; protected set; } = new List<State>();


        public Entity_DnD_5e(string name       = "[Unknown Entity]"
                            ,string race       = null
                            ,string className  = null
                            ,int    lvl        = 1
                            ,int    exp        = 0
                            ,int    hpMax      = 1
                            ,State? stats      = null
                            ,State? modifiers  = null
                            ,Inventaire_DnD_5e Inventaire = null
                            )
                            : base(name
                                  ,lvl
                                  ,exp
                                  ,hpMax
                                  ,Inventaire ?? new Inventaire_DnD_5e()
                                  )
        {
            this.Race       = race;
            this.Class      = className;
            this.Stats      = stats      ?? new State(10, 10, 10, 10, 10, 10);
            this.Modifiers  = modifiers  ?? Stats.mod;

        }

        public override void RemoveEtat(Etat_DnD_5e etat)
        {
            base.RemoveEtat(etat);
        }

        public override void AddEtat(Etat_DnD_5e etat)
        {
            base.AddEtat(etat);
        }

        public int Roll(DnDRollType rollType)
        {
            switch (rollType)
            {
                // --- Caractéristiques brutes ---
                case DnDRollType.Strength:         return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Str);
                case DnDRollType.Dexterity:        return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Dex);
                case DnDRollType.Constitution:     return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Con);
                case DnDRollType.Intelligence:     return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Int);
                case DnDRollType.Wisdom:           return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Wis);
                case DnDRollType.Charisma:         return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Cha);

                // --- Jets de sauvegarde ---
                case DnDRollType.StrengthSave:     return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Str);
                case DnDRollType.DexteritySave:    return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Dex);
                case DnDRollType.ConstitutionSave: return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Con);
                case DnDRollType.IntelligenceSave: return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Int);
                case DnDRollType.WisdomSave:       return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Wis);
                case DnDRollType.CharismaSave:     return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Cha);

                // --- Compétences : Force ---
                case DnDRollType.Athletics:        return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Str);

                // --- Compétences : Dextérité ---
                case DnDRollType.Acrobatics:       return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Dex);
                case DnDRollType.SleightOfHand:    return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Dex);
                case DnDRollType.Stealth:          return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Dex);

                // --- Compétences : Intelligence ---
                case DnDRollType.Arcana:           return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Int);
                case DnDRollType.History:          return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Int);
                case DnDRollType.Investigation:    return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Int);
                case DnDRollType.Nature:           return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Int);
                case DnDRollType.Religion:         return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Int);

                // --- Compétences : Sagesse ---
                case DnDRollType.AnimalHandling:   return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Wis);
                case DnDRollType.Insight:          return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Wis);
                case DnDRollType.Medicine:         return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Wis);
                case DnDRollType.Perception:       return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Wis);
                case DnDRollType.Survival:         return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Wis);

                // --- Compétences : Charisme ---
                case DnDRollType.Deception:        return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Cha);
                case DnDRollType.Intimidation:     return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Cha);
                case DnDRollType.Performance:      return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Cha);
                case DnDRollType.Persuasion:       return new Dice { Number = 1, Value = 20 }.Roll(this.Modifiers.Cha);

                default:
                    return new Dice { Number = 1, Value = 20 }.Roll();
            }
        }
    }
}
