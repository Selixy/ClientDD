using System.Collections.Generic;
using System;

namespace DnD.DnD_5e
{
    public class Class_DnD_5e
    {
        // ──────────────── Identité et lien ────────────────
        public string         Name        { get; protected set; }
        public Entity_DnD_5e  SelfEntity  { get; protected set; }


        // ──────────────── Caractéristiques générales ────────────────
        public int            Lvl         { get; protected set; }
        public int            HitDie      { get; protected set; }
        public ClassArchetype Archetype   { get; protected set; }


        // ──────────────── Competances et magie ────────────────
        public SpellcastingStyle SpellcastingStyle         { get; protected set; }
        public int[]             NbSpell                   { get; protected set; }          // [cantrips, spells]
        public List<Skill>       Spell                     { get; protected set; }
        public List<Skill>       Skill                     { get; protected set; }
        public int[][]           SpecialCasterProgression  { get; protected set; }
        public StateEnum         CastingStat               { get; protected set; }


        // ──────────────── Progression et montée de niveau ────────────────
        public LvlUpClass[][] LevelUps { get; protected set; }


        // ──────────────── Choix de compétences ou capacités ────────────────
        public List<int> Choix { get; protected set; }



        // ──────────────── Constructeur ────────────────
        public Class_DnD_5e(Entity_DnD_5e     SelfEntity
                           ,string            Name                      = "[Unknown Class DnD 5e]"
                           ,int               Lvl                       = 1
                           ,int               HitDie                    = 8
                           ,List<int>         Choix                     = null
                           ,int[]             NbSpell                   = null
                           ,List<Skill>       Spell                     = null
                           ,List<Skill>       Skill                     = null
                           ,SpellcastingStyle SpellcastingStyle         = SpellcastingStyle.None
                           ,ClassArchetype    Archetype                 = ClassArchetype.None
                           ,StateEnum         CastingStat               = StateEnum.Intelligence
                           ,int[][]           SpecialCasterProgression  = null
                           ,LvlUpClass[][]    LevelUps                  = null
                           )
        {
            this.SelfEntity               = SelfEntity;
            this.Name                     = Name;
            this.HitDie                   = HitDie;
            this.Lvl                      = Lvl;
            this.Choix                    = Choix;
            this.Archetype                = Archetype;
            this.LevelUps                 = LevelUps;
            this.NbSpell                  = NbSpell ?? new int[2];
            this.Spell                    = Spell;
            this.Skill                    = Skill;
            this.SpellcastingStyle        = SpellcastingStyle;
            this.SpecialCasterProgression = SpecialCasterProgression;
            this.CastingStat              = CastingStat;
        }



        // ──────────────── Méthodes ────────────────
        public void Lvl_Up()
        {
            var hitDie = new Dice(1, this.HitDie, DiceType.HitDie);
            var (total, rolls) = hitDie.Roll();
            this.SelfEntity.Lvl_Up(total);
            this.Lvl++;
        }

        public void TogglePreparedSpellByName(string SpellName)
        {
            if (SpellcastingStyle != SpellcastingStyle.Prepared
            && SpellcastingStyle != SpellcastingStyle.Grimoire)
                return;

            Skill spellToToggle = this.Spell?.Find(s => s.Name == SpellName);
            if (spellToToggle == null)
                return;

            List<Skill> preparedList = Archetype switch
            {
                ClassArchetype.SpecialCaster => this.SelfEntity?.Spell_SpecialCaster,
                _                            => this.SelfEntity?.Spell_Caster
            };

            if (preparedList == null)
                return;

            if (preparedList.Contains(spellToToggle))
                preparedList.Remove(spellToToggle);
            else if (preparedList.Count < this.GetMaxPreparedSpell)
                preparedList.Add(spellToToggle);
        }

        public int GetMaxPreparedSpell
        {
            get
            {
                if (SpellcastingStyle != SpellcastingStyle.Prepared
                && SpellcastingStyle != SpellcastingStyle.Grimoire)
                    return 0;

                // Utilise la stat principale du lanceur
                int statMod = SpellcastingStyle switch
                {
                    SpellcastingStyle.Prepared => GetCastingStatMod,
                    SpellcastingStyle.Grimoire => GetCastingStatMod,
                    _ => 0
                };

                return Math.Max(1, statMod + this.Lvl);
            }
        }

        public int GetCastingStatMod
        {
            get
            {
                return this.CastingStat switch
                {
                    StateEnum.Strength     => this.SelfEntity.Modifiers.Str,
                    StateEnum.Dexterity    => this.SelfEntity.Modifiers.Dex,
                    StateEnum.Constitution => this.SelfEntity.Modifiers.Con,
                    StateEnum.Intelligence => this.SelfEntity.Modifiers.Int,
                    StateEnum.Wisdom       => this.SelfEntity.Modifiers.Wis,
                    StateEnum.Charisma     => this.SelfEntity.Modifiers.Cha,
                    _ => 0
                };
            }
        }
    }
}