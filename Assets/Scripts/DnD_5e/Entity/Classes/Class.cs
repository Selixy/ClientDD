namespace DnD.DnD_5e
{
    public enum ClassArchetype
    {
        None,
        FullCaster,
        HalfCaster,
        Martial,
    }

    public class Class_DnD_5e 
    {
        public string            Name        { get; protected set; }
        public int               Lvl         { get; protected set; }
        public ClassArchetype    Archetype   { get; protected set; }
        public int               HitDie      { get; protected set; }
        public Entity_DnD_5e     SelfEntity  { get; protected set; }         

        public Class_DnD_5e(Entity_DnD_5e    SelfEntity
                           ,string           Name       = "[Unknown Entity]"
                           ,int              Lvl        = 1
                           ,ClassArchetype   Archetype  = ClassArchetype.None
                           )
        {
            this.Name       = Name;
            this.Lvl        =  Lvl;
            this.SelfEntity = SelfEntity;
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