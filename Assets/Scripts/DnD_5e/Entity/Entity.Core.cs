using System.Collections.Generic;
using System.Linq;
using System;


namespace DnD.DnD_5e
{ 
    public partial class Entity_DnD_5e : Entity
    {
        public string               Race              { get; protected set; }
        public List<Class_DnD_5e>   Classes           { get; protected set; }
        public State                Stats             { get; protected set; }
        public int                  AC                { get; protected set; }
        public int                  CritBonus         { get; protected set; }
        public State                Modifiers         { get; protected set; }
        public int                  Maitrise          { get; protected set; }
        public int                  Initiative        { get; protected set; }
        public int                  WalkSpeed         { get; protected set; }
        public int                  ExtraAtaque       { get; protected set; }
        public int                  ExtraAction       { get; protected set; }
        public float                SpeedFactor       { get; protected set; }
        public int[]                SpellEmplasement  { get; protected set; }
        public int[]                SpecialRessources { get; protected set; }

        public Dictionary<DnDRollType, SkillData> SkillMastery  { get; protected set; }
        public Dictionary<DamageType, int?> DamageResistance    { get; protected set; }

        public ActivContext      fightContext    { get; set; }
        public List<State>            Debuffs    { get; protected set; } = new List<State>();
        public int[]                  Actions    { get; protected set; } = new int[5] {30, 1, 1, 1, 0};  // {Deplacement, Action, BonusAction, Reaction, ExtraAtaque}

        public List<Skill>   Spell_FillCaster    { get; protected set; }
        public List<Skill>   Spell_HalfCaster    { get; protected set; }
        public List<Skill>   Spell_SpecialCaster { get; protected set; }
        public List<Skill>   Skill_InFignter     { get; protected set; }


        public Entity_DnD_5e(string             name       = "[Unknown Entity]"
                            ,string             race       = null
                            ,List<Class_DnD_5e> classes    = null
                            ,int                lvl        = 1
                            ,int                exp        = 0
                            ,int                hpMax      = 1
                            ,int?               curHp      = null
                            ,int?               maitrise   = null
                            ,State?             stats      = null
                            ,int                AC         = 10
                            ,int                WalkSpeed  = 30
                            ,int                CritBonus  = 0
                            ,State?             modifiers  = null
                            ,Dictionary<DnDRollType, SkillData> skillMastery = null
                            ,Dictionary<DamageType, int?>   DamageResistance = null
                            ,Inventaire_DnD_5e              Inventaire       = null
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
            this.Classes          = classes;
            this.Stats            = stats            ?? new State(10, 10, 10, 10, 10, 10);
            this.AC               = AC;
            this.WalkSpeed        = WalkSpeed;
            this.CritBonus        = CritBonus;
            this.Modifiers        = modifiers        ?? Stats.mod;
            this.Maitrise         = maitrise         ?? GetProficiencyBonus(base.Lvl);
            this.SkillMastery     = skillMastery     ?? null;
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

        public void Lvl_Up(int hpAddMax)
        {
            base.HpMax += hpAddMax;
            base.Heal(hpAddMax);
            base.Lvl_Up();
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
