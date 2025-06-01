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
    }

    public class Entity_DnD_5e
    {
        public string            Name      { get; private set; }
        public string            Race      { get; private set; }
        public string            Class     { get; private set; }

        public int               Lvl       { get; private set; }
        public int               Exp       { get; private set; }
        public int               HpMax     { get; private set; }
        public State             Stats     { get; private set; }
        public State             Modifiers { get; private set; }

        public int               CurHp     { get; private set; }
        public int               HpBonus   { get; private set; }

        public List<State>       Debuffs   { get; private set; } = new List<State>();

        public int               Gold      { get; private set; }
        public List<Item_DnD_5e> Inventory { get; private set; } = new List<Item_DnD_5e>();


        public Entity_DnD_5e(string Name      = "[Unknown Entity]"
                            ,string Race      = null
                            ,string Class     = null
                            ,int    Lvl       = 1
                            ,int    Exp       = 0

                            ,int    HpMax     = 1
                            ,State? Stats     = null
                            ,State? Modifiers = null
                            )
        {
            this.Name  = Name;
            this.Race  = Race;
            this.Class = Class;
            this.Lvl   = Lvl;
            this.Exp   = Exp;
        
            Stats     = Stats     ?? new State(10, 10, 10, 10, 10, 10);
            Modifiers = Modifiers ?? new State(0, 0, 0, 0, 0, 0);

        }

        // Méthodes communes à toutes les entités 
        public virtual void TakeDamage(int amount)
        {
            CurHp -= amount;
            if (CurHp < 0) CurHp = 0;
        }

    }
}
