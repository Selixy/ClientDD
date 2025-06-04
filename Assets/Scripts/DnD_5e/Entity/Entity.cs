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

    public class Entity_DnD_5e : Entity
    {
        public string            Race      { get; private set; }
        public string            Class     { get; private set; }
        public State             Stats     { get; private set; }
        public State             Modifiers { get; private set; }

        public int               Gold      { get; private set; }

        public List<Item_DnD_5e> Inventory { get; private set; } = new List<Item_DnD_5e>();
        public List<State>       Debuffs   { get; private set; } = new List<State>();

        public Entity_DnD_5e(string name       = "[Unknown Entity]",
                             string race       = null,
                             string className  = null,
                             int    lvl        = 1,
                             int    exp        = 0,
                             int    hpMax      = 1,
                             State? stats      = null,
                             State? modifiers  = null)
            : base(name, lvl, exp, hpMax)
        {
            Race      = race;
            Class     = className;
            Stats     = stats     ?? new State(10, 10, 10, 10, 10, 10);
            Modifiers = modifiers ?? new State(0, 0, 0, 0, 0, 0);
        }
    }
}
