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
        // Jet de caractéristique
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma,

        // Jets de sauvegarde (Saving Throws)
        StrengthSave,
        DexteritySave,
        ConstitutionSave,
        IntelligenceSave,
        WisdomSave,
        CharismaSave,

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

        public (int total, List<(int value, bool kept)> rolls) Roll(DnDRollType rollType, int bonus = 0, int isAdvantage = 0)
        {
            int mod = rollType switch
            {
                // --- Caractéristiques brutes ---
                DnDRollType.Strength         => this.Modifiers.Str,
                DnDRollType.Dexterity        => this.Modifiers.Dex,
                DnDRollType.Constitution     => this.Modifiers.Con,
                DnDRollType.Intelligence     => this.Modifiers.Int,
                DnDRollType.Wisdom           => this.Modifiers.Wis,
                DnDRollType.Charisma         => this.Modifiers.Cha,

                // --- Jets de sauvegarde ---
                DnDRollType.StrengthSave     => this.Modifiers.Str,
                DnDRollType.DexteritySave    => this.Modifiers.Dex,
                DnDRollType.ConstitutionSave => this.Modifiers.Con,
                DnDRollType.IntelligenceSave => this.Modifiers.Int,
                DnDRollType.WisdomSave       => this.Modifiers.Wis,
                DnDRollType.CharismaSave     => this.Modifiers.Cha,

                // --- Compétences : Force ---
                DnDRollType.Athletics        => this.Modifiers.Str,

                // --- Compétences : Dextérité ---
                DnDRollType.Acrobatics       => this.Modifiers.Dex,
                DnDRollType.SleightOfHand    => this.Modifiers.Dex,
                DnDRollType.Stealth          => this.Modifiers.Dex,

                // --- Compétences : Intelligence ---
                DnDRollType.Arcana           => this.Modifiers.Int,
                DnDRollType.History          => this.Modifiers.Int,
                DnDRollType.Investigation    => this.Modifiers.Int,
                DnDRollType.Nature           => this.Modifiers.Int,
                DnDRollType.Religion         => this.Modifiers.Int,

                // --- Compétences : Sagesse ---
                DnDRollType.AnimalHandling   => this.Modifiers.Wis,
                DnDRollType.Insight          => this.Modifiers.Wis,
                DnDRollType.Medicine         => this.Modifiers.Wis,
                DnDRollType.Perception       => this.Modifiers.Wis,
                DnDRollType.Survival         => this.Modifiers.Wis,

                // --- Compétences : Charisme ---
                DnDRollType.Deception        => this.Modifiers.Cha,
                DnDRollType.Intimidation     => this.Modifiers.Cha,
                DnDRollType.Performance      => this.Modifiers.Cha,
                DnDRollType.Persuasion       => this.Modifiers.Cha,

                _ => 0
            };

            return new Dice { Number = 1, Value = 20 }.Roll(mod + bonus, isAdvantage);
        }
    }
}
