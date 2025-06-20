using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public enum ClassArchetype
    {
        None,
        FullCaster,
        HalfCaster,
        Special,
        Martial,
    }

    public enum SpellcastingStyle
    {
        None,          // Pas de magie (Barbare)
        Known,         // Liste fixe de sorts connus (Sorcerer, Bard, Warlock, Ranger)
        Prepared,      // Préparés à partir d’une liste fixe (Cleric, Druid, Paladin)
        Grimoire       // Préparés depuis un grimoire (Wizard)
    }

    public class Class_DnD_5e 
    {
        public string              Name                       { get; protected set; }
        public int                 Lvl                        { get; protected set; }
        public ClassArchetype      Archetype                  { get; protected set; }
        public int                 HitDie                     { get; protected set; }
        public Entity_DnD_5e       SelfEntity                 { get; protected set; }
        public List<int>           Choix                      { get; protected set; }
        public int[]               NbSpell                    { get; protected set; }
        public SpellcastingStyle   SpellcastingStyle          { get; protected set; }
        public List<Spell>         Spell                      { get; protected set; }

        public LvlUpClass[][]      LevelUps                   { get; protected set; }
        public int[][]             SpecialCasterProgression   { get; protected set; }

        public Class_DnD_5e(Entity_DnD_5e     SelfEntity
                           ,string            Name                      = "[Unknown Class DnD 5e]"
                           ,int               Lvl                       = 1
                           ,int               HitDie                    = 8
                           ,List<int>         Choix                     = null
                           ,int[]             NbSpell                   = null
                           ,List<Spell>       Spell                     = null
                           ,SpellcastingStyle SpellcastingStyle         = SpellcastingStyle.None
                           ,ClassArchetype    Archetype                 = ClassArchetype.None
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
            this.SpellcastingStyle        = SpellcastingStyle;
            this.SpecialCasterProgression = SpecialCasterProgression;
        }

        public void Lvl_Up()
        {
            var hitDie = new Dice(1, this.HitDie, DiceType.HitDie);
            var (total, rolls) = hitDie.Roll();
            this.SelfEntity.Lvl_Up(total);
            this.Lvl ++;
        }
    }
}