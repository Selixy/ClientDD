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

    public class Class_DnD_5e 
    {
        public string            Name               { get; protected set; }
        public int               Lvl                { get; protected set; }
        public ClassArchetype    Archetype          { get; protected set; }
        public int               HitDie             { get; protected set; }
        public Entity_DnD_5e     SelfEntity         { get; protected set; }         
        public List<int>         Choix              { get; protected set; }

        public Dictionary<int, List<LvlUpClass>>      LevelUps { get; protected set; } = new();
        public Dictionary<int, int[]> SpecialCasterProgression { get; protected set; } = new();

        public Class_DnD_5e(Entity_DnD_5e    SelfEntity
                           ,string           Name       = "[Unknown Class DnD 5e]"
                           ,int              Lvl        = 1
                           ,List<int>        Choix      = null
                           ,ClassArchetype                      Archetype  = ClassArchetype.None
                           ,Dictionary<int, int[]>              SpecialCasterProgression = null
                           ,Dictionary<int, List<LvlUpClass>>   LevelUps   = null
                           )
        {
            this.SelfEntity = SelfEntity;
            this.Name       = Name;
            this.Lvl        = Lvl;
            this.Choix      = Choix;
            this.Archetype  = Archetype;
            this.SpecialCasterProgression = SpecialCasterProgression;
            this.LevelUps   = LevelUps;
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