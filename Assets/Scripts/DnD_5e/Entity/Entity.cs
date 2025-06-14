using System.Collections.Generic;
using System;

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

    public enum ContextRoll
    {
        Unknown,
        AttackRoll,
        DamageRoll,
        SauvegardeRoll,
        AbilityRoll,
    }

    public struct SkillData
    {
        public int MasteryLevel; // 0 = pas maîtrisé, 1 = maîtrise, 2 = expertise
        public int FlatBonus;    // bonus fixe (ex: objet, race...)

        public SkillData(int masteryLevel, int flatBonus = 0)
        {
            MasteryLevel = masteryLevel;
            FlatBonus = flatBonus;
        }
    }

    public class Entity_DnD_5e : Entity<Item_DnD_5e, Etat_DnD_5e>
    {
        public string            Race           { get; protected set; }
        public string            Class          { get; protected set; }
        public State             Stats          { get; protected set; }
        public State             Modifiers      { get; protected set; }
        public int               Maitrise       { get; protected set; }
        public Dictionary<DnDRollType, SkillData> SkillMastery { get; protected set; }


        public ActivContext      fightContext   { get; set; }
        public List<State>           Debuffs    { get; protected set; } = new List<State>();


        public Entity_DnD_5e(string name         = "[Unknown Entity]"
                            ,string race         = null
                            ,string className    = null
                            ,int    lvl          = 1
                            ,int    exp          = 0
                            ,int    hpMax        = 1
                            ,int?   curHp        = null
                            ,int?   maitrise     = null
                            ,State? stats        = null
                            ,State? modifiers    = null
                            ,Dictionary<DnDRollType, SkillData> skillMastery = null
                            ,Inventaire_DnD_5e Inventaire = null
                            )
                            : base(name
                                  ,lvl
                                  ,exp
                                  ,hpMax
                                  ,curHp      ?? hpMax
                                  ,Inventaire ?? new Inventaire_DnD_5e()
                                  )
        {
            this.Race         = race;
            this.Class        = className;
            this.Stats        = stats        ?? new State(10, 10, 10, 10, 10, 10);
            this.Modifiers    = modifiers    ?? Stats.mod;
            this.Maitrise     = maitrise     ?? GetProficiencyBonus(base.Lvl);
            this.SkillMastery = skillMastery ?? null;
        }

        public static Dictionary<DnDRollType, SkillData> CompleteSkillDictionary(Dictionary<DnDRollType, SkillData> input)
        {
            var output = new Dictionary<DnDRollType, SkillData>();

            foreach (DnDRollType type in System.Enum.GetValues(typeof(DnDRollType)))
            {
                // Ignore les caractéristiques brutes
                if (type is DnDRollType.Strength
                         or DnDRollType.Dexterity
                         or DnDRollType.Constitution
                         or DnDRollType.Intelligence
                         or DnDRollType.Wisdom
                         or DnDRollType.Charisma
                         )
                {
                    continue;
                }

                if (input != null && input.TryGetValue(type, out var data))
                    output[type] = data;
                else
                    output[type] = new SkillData(0, 0); // valeur par défaut
            }

            return output;
        }

        public static int GetProficiencyBonus(int level)
        {
            if (level >= 17) return 6;
            if (level >= 13) return 5;
            if (level >=  9) return 4;
            if (level >=  5) return 3;
            return 2;
        }

        public override void RemoveEtat(Etat_DnD_5e etat)
        {
            base.RemoveEtat(etat);
        }

        public override void AddEtat(Etat_DnD_5e etat)
        {
            base.AddEtat(etat);
        }

        public (int masteryBonus, int flatBonus) GetSkillBonuses(DnDRollType type)
        {
            if (SkillMastery != null && SkillMastery.TryGetValue(type, out var data))
            {
                int masteryLevel = Math.Clamp(data.MasteryLevel, 0, 2);
                int mastery = this.Maitrise * masteryLevel;
                return (mastery, data.FlatBonus);
            }

            return (0, 0);
        }

        public (int total, List<(int value, bool kept)> rolls) Roll(DnDRollType rollType, int bonus = 0, int AddAdvantage = 0, ContextRoll Context = ContextRoll.Unknown)
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

            int isAdvantage = 0;
            isAdvantage += AddAdvantage;

            var (mastery, flat) = GetSkillBonuses(rollType);
            bonus += mastery + flat;

            return new Dice { Number = 1, Value = 20 }.Roll(mod + bonus, isAdvantage);
        }
    }
}
