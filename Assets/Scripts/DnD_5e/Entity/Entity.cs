using System.Collections.Generic;
using System.Linq;
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
        None,

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

    public class Entity_DnD_5e : Entity
    {
        public string            Race           { get; protected set; }
        public string            Class          { get; protected set; }
        public State             Stats          { get; protected set; }
        public int               AC             { get; protected set; }
        public int               CritBonus      { get; protected set; }
        public State             Modifiers      { get; protected set; }
        public int               Maitrise       { get; protected set; }

        public Dictionary<DnDRollType, SkillData> SkillMastery  { get; protected set; }
        public Dictionary<DamageType, int?> DamageResistance    { get; protected set; }

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
                            ,int    AC           = 10
                            ,int    CritBonus    = 0
                            ,State? modifiers    = null
                            ,Dictionary<DnDRollType, SkillData> skillMastery = null
                            ,Dictionary<DamageType, int?> DamageResistance = null
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
            this.Race             = race;
            this.Class            = className;
            this.Stats            = stats        ?? new State(10, 10, 10, 10, 10, 10);
            this.AC               = AC;
            this.CritBonus        = CritBonus;
            this.Modifiers        = modifiers    ?? Stats.mod;
            this.Maitrise         = maitrise     ?? GetProficiencyBonus(base.Lvl);
            this.SkillMastery     = skillMastery ?? null;
            this.DamageResistance = DamageResistance ?? null;
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

        public override void RemoveEtat(Etat etat)
        {
            base.RemoveEtat(etat);
        }

        public override void AddEtat(Etat etat)
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

        public (int total, int natural) Roll(DnDRollType rollType, int bonus = 0, int AddAdvantage = 0, ContextRoll Context = ContextRoll.Unknown)
        {
            int mod = rollType switch
            {
                // caractéristiques
                DnDRollType.Strength         => Modifiers.Str,
                DnDRollType.Dexterity        => Modifiers.Dex,
                DnDRollType.Constitution     => Modifiers.Con,
                DnDRollType.Intelligence     => Modifiers.Int,
                DnDRollType.Wisdom           => Modifiers.Wis,
                DnDRollType.Charisma         => Modifiers.Cha,

                // jets de sauvegarde
                DnDRollType.StrengthSave     => Modifiers.Str,
                DnDRollType.DexteritySave    => Modifiers.Dex,
                DnDRollType.ConstitutionSave => Modifiers.Con,
                DnDRollType.IntelligenceSave => Modifiers.Int,
                DnDRollType.WisdomSave       => Modifiers.Wis,
                DnDRollType.CharismaSave     => Modifiers.Cha,

                // compétences
                DnDRollType.Athletics        => Modifiers.Str,
                DnDRollType.Acrobatics       => Modifiers.Dex,
                DnDRollType.SleightOfHand    => Modifiers.Dex,
                DnDRollType.Stealth          => Modifiers.Dex,
                DnDRollType.Arcana           => Modifiers.Int,
                DnDRollType.History          => Modifiers.Int,
                DnDRollType.Investigation    => Modifiers.Int,
                DnDRollType.Nature           => Modifiers.Int,
                DnDRollType.Religion         => Modifiers.Int,
                DnDRollType.AnimalHandling   => Modifiers.Wis,
                DnDRollType.Insight          => Modifiers.Wis,
                DnDRollType.Medicine         => Modifiers.Wis,
                DnDRollType.Perception       => Modifiers.Wis,
                DnDRollType.Survival         => Modifiers.Wis,
                DnDRollType.Deception        => Modifiers.Cha,
                DnDRollType.Intimidation     => Modifiers.Cha,
                DnDRollType.Performance      => Modifiers.Cha,
                DnDRollType.Persuasion       => Modifiers.Cha,

                _ => 0
            };

            var (mastery, flat) = GetSkillBonuses(rollType);
            bonus += mastery + flat;

            var d20 = new Dice(1, 20, DiceType.Neutre);
            var (total, rolls) = d20.Roll(mod + bonus, AddAdvantage);

            var natural = rolls.FirstOrDefault(r => r.kept).value;

            return (total, natural);
        }

        public virtual (int total, int natural) RequestJDS(DnDRollType rollType, ContextRoll Context)
        {
            return this.Roll(rollType, Context: Context);
        }
        
        
        public Damage DamageRoll(List<DamageComponent> diceList, bool isMagical = false)
        {
            Damage result = new Damage { Magique = isMagical };

            foreach (var dd in diceList)
            {
                int rolled = dd.Roll();;

                switch (dd.Type)
                {
                    case DamageType.Contondant:  result.Contondant += rolled; break;
                    case DamageType.Perforant:   result.Perforant  += rolled; break;
                    case DamageType.Tranchant:   result.Tranchant  += rolled; break;
                    case DamageType.Force:       result.Force      += rolled; break;

                    case DamageType.Feu:         result.Feu        += rolled; break;
                    case DamageType.Froid:       result.Froid      += rolled; break;
                    case DamageType.Foudre:      result.Foudre     += rolled; break;
                    case DamageType.Tonnerre:    result.Tonnerre   += rolled; break;

                    case DamageType.Acide:       result.Acide      += rolled; break;
                    case DamageType.Poison:      result.Poison     += rolled; break;

                    case DamageType.Radiant:     result.Radiant    += rolled; break;
                    case DamageType.Nécrotique:  result.Nécrotique += rolled; break;
                    case DamageType.Psychique:   result.Psychique  += rolled; break;
                    case DamageType.Heal:        result.Heal       += rolled; break;
                }
            }
            return result;
        }

        public void ApplyDamage(Damage damage)
        {
            int total = 0;

            // Parcourt tous les champs de la struct Damage (Contondant, Feu, etc.)
            foreach (var entry in damage.GetType().GetFields())
            {
                // On ne traite que les champs de type int (ignore par ex. Magique)
                if (entry.FieldType != typeof(int)) continue;

                // Récupère la valeur brute du champ (par exemple : 6 feu)
                int baseValue = (int)entry.GetValue(damage);
                if (baseValue <= 0) continue; // Ignore les dégâts nuls

                // Essaie de convertir le nom du champ en DamageType (ex: "Feu" → DamageType.Feu)
                if (!Enum.TryParse<DamageType>(entry.Name, ignoreCase: true, out var type))
                    continue;

                // Récupère la résistance associée à ce type de dégât
                int? resistance = DamageResistance != null && DamageResistance.TryGetValue(type, out var val) ? val : 0;

                // Si la résistance est null → immunité totale → on ignore les dégâts
                if (resistance == null)
                    continue;

                // Applique le facteur : 1 = moitié, 2 = quart, -1 = double, etc.
                double modifier = Math.Pow(0.5, resistance.Value);

                // Ajoute les dégâts modifiés au total
                total += (int)Math.Round(baseValue * modifier);
            }

            // Applique le total de dégâts à l'entité
            base.TakeDamage(total);
        }

    }
}
