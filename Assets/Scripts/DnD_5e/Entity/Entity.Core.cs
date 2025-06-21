using System.Collections.Generic;
using System.Linq;
using System;


namespace DnD.DnD_5e
{ 
    public partial class Entity_DnD_5e : Entity
    {
        // ──────────────── Identité de base ────────────────
        public string               Race                 { get; protected set; }
        public List<Class_DnD_5e>   Classes              { get; protected set; }


        // ──────────────── Caractéristiques ────────────────
        public State                Stats                { get; protected set; }
        public State                Modifiers            { get; protected set; }
        public int                  Maitrise             { get; protected set; }


        // ──────────────── Défense / Combat ────────────────
        public int                  AC                   { get; protected set; }
        public int                  CritBonus            { get; protected set; }
        public int                  Initiative           { get; protected set; }


        // ──────────────── Mouvement et actions ────────────────
        public int                  WalkSpeed            { get; protected set; }
        public float                SpeedFactor          { get; protected set; }
        public int                  ExtraAtaque          { get; protected set; }
        public int                  ExtraAction          { get; protected set; }
        public int[]                Actions              { get; protected set; } = new int[5] {30, 1, 1, 1, 0};


        // ──────────────── Sorts et compétences ────────────────
        public List<Skill>   Spell_Caster                  { get; protected set; }
        public List<Skill>   UnpreparedSpell_Caster        { get; protected set; }
        public List<Skill>   UnpreparedSpell_SpecialCaster { get; protected set; }
        public List<Skill>   Spell_SpecialCaster           { get; protected set; }
        public List<Skill>   Skill_InFignter               { get; protected set; }
        public List<Skill>   Special_Ability               { get; protected set; }


        // ──────────────── Ressources ────────────────
        public Dictionary<string, int[]> Ressources      { get; protected set; }
        public Dictionary<string, int[]> RessourcesMax   { get; protected set; }


        // ──────────────── Système de compétences et résistance ────────────────
        public Dictionary<DnDRollType, SkillData> SkillMastery     { get; protected set; }
        public Dictionary<DamageType, int?>       DamageResistance { get; protected set; }


        // ──────────────── État de combat ────────────────
        public ActivContext         fightContext      { get; set; }
        public List<State>          Debuffs           { get; protected set; } = new List<State>();



        // ──────────────── Constructeur ────────────────
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
                            ,Dictionary<DnDRollType, SkillData> skillMastery     = null
                            ,Dictionary<DamageType, int?>       DamageResistance = null
                            ,Inventaire_DnD_5e                  Inventaire       = null
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





        // ──────────────── Fonctions statiques utilitaires ────────────────

        public static Dictionary<DnDRollType, SkillData> CompleteSkillDictionary(Dictionary<DnDRollType, SkillData> input)
        {
            var output = new Dictionary<DnDRollType, SkillData>();

            foreach (DnDRollType type in Enum.GetValues(typeof(DnDRollType)))
            {
                // Ignore les caractéristiques brutes
                if (type is DnDRollType.Strength
                        or DnDRollType.Dexterity
                        or DnDRollType.Constitution
                        or DnDRollType.Intelligence
                        or DnDRollType.Wisdom
                        or DnDRollType.Charisma)
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

        public int[] GetSpellEmplasementMax()
        {
            int fullCasterLevel  = 0;
            int halfCasterLevel  = 0;
            int thirdCasterLevel = 0;

            foreach (var c in this.Classes)
            {
                switch (c.Archetype)
                {
                    case ClassArchetype.FullCaster:
                        fullCasterLevel += c.Lvl;
                        break;
                    case ClassArchetype.HalfCaster:
                        halfCasterLevel += c.Lvl;
                        break;
                    case ClassArchetype.ThirdCaster:
                        thirdCasterLevel += c.Lvl;
                        break;
                }
            }

            int totalCasterLevel =
                fullCasterLevel
                + (int)Math.Floor(halfCasterLevel / 2.0)
                + (int)Math.Floor(thirdCasterLevel / 3.0);

            totalCasterLevel = Math.Clamp(totalCasterLevel, 0, 20);

            // Récupère les emplacements standards
            var spellSlots = CasterInfo.CasterSlots[totalCasterLevel];

            int[] result = new int[9];
            for (int i = 0; i < 9; i++)
                result[i] = spellSlots?[i] ?? 0;

            // Met à jour RessourcesMax
            RessourcesMax["SpellSlots"] = result;
            return result;
        }

        public void Add_SpecialAbility(Skill s)
        {
            if(!(this.Special_Ability.Contains(s)))
                this.Special_Ability.Add(s);
        }

        public void Add_SpellCaster(Skill s)
        {
            if(!(this.Spell_Caster.Contains(s)))
                this.Spell_Caster.Add(s);
        }

        public void Add_UnpreparedSpellCaster(Skill s)
        {
            if(!(this.UnpreparedSpell_Caster.Contains(s)))
                this.UnpreparedSpell_Caster.Add(s);
        }

        public void Add_UnpreparedSpellSpecialCaster(Skill s)
        {
            if(!(this.UnpreparedSpell_SpecialCaster.Contains(s)))
                this.UnpreparedSpell_SpecialCaster.Add(s);
        }

        public void Add_SpellSpecialCaster(Skill s)
        {
            if(!(this.Spell_SpecialCaster.Contains(s)))
                this.Spell_SpecialCaster.Add(s);
        }

        public void Add_SkillInFignter(Skill s)
        {
            if(!(this.Skill_InFignter.Contains(s)))
                this.Skill_InFignter.Add(s);
        }
        
        public void Remove_SpecialAbility(Skill s)
        {
            if(this.Special_Ability.Contains(s))
                this.Special_Ability.Remove(s);
        }

        public void Remove_SpellCaster(Skill s)
        {
            if(this.Spell_Caster.Contains(s))
                this.Spell_Caster.Remove(s);
        }

        public void Remove_UnpreparedSpellCaster(Skill s)
        {
            if(this.UnpreparedSpell_Caster.Contains(s))
                this.UnpreparedSpell_Caster.Remove(s);
        }

        public void Remove_UnpreparedSpellSpecialCaster(Skill s)
        {
            if(this.UnpreparedSpell_SpecialCaster.Contains(s))
                this.UnpreparedSpell_SpecialCaster.Remove(s);
        }

        public void Remove_SpellSpecialCaster(Skill s)
        {
            if(this.Spell_SpecialCaster.Contains(s))
                this.Spell_SpecialCaster.Remove(s);
        }

        public void Remove_SkillInFignter(Skill s)
        {
            if(this.Skill_InFignter.Contains(s))
                this.Skill_InFignter.Remove(s);
        }



        // ──────────────── Gestion des états ────────────────

        public override void AddEtat(Etat etat)
        {
            base.AddEtat(etat);
        }

        public override void RemoveEtat(Etat etat)
        {
            base.RemoveEtat(etat);
        }



        // ──────────────── Progression / niveau ────────────────

        public void Lvl_Up(int hpAddMax)
        {
            base.HpMax += hpAddMax;
            base.Heal(hpAddMax);
            base.Lvl_Up();
        }



        // ──────────────── Calcul des dégâts avec résistances ────────────────

        public void ApplyDamage(Damage damage)
        {
            int total = 0;

            foreach (var entry in damage.GetType().GetFields())
            {
                if (entry.FieldType != typeof(int)) continue;

                int baseValue = (int)entry.GetValue(damage);
                if (baseValue <= 0) continue;

                if (!Enum.TryParse<DamageType>(entry.Name, ignoreCase: true, out var type))
                    continue;

                int? resistance = DamageResistance != null && DamageResistance.TryGetValue(type, out var val) ? val : 0;

                if (resistance == null)
                    continue;

                double modifier = Math.Pow(0.5, resistance.Value);

                total += (int)Math.Round(baseValue * modifier);
            }

            base.TakeDamage(total);
        }

        public void ConsumeResources(Dictionary<string, int[]> resourceCosts)
        {
            if (resourceCosts == null)
                return;

            foreach (var pair in resourceCosts)
            {
                string key = pair.Key;
                int[] costs = pair.Value;

                if (!Ressources.ContainsKey(key))
                    continue;

                int[] values = Ressources[key];

                for (int i = 0; i < costs.Length; i++)
                {
                    if (costs[i] <= 0) continue;

                    if (i >= values.Length)
                        continue; // ignorer l’index inexistant

                    values[i] = Math.Max(0, values[i] - costs[i]);
                }

                // Réenregistre le tableau modifié
                Ressources[key] = values;
            }
        }
    }
}
