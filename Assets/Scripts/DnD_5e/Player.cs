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

    public class PlayerDND5e
    {
        public string Name     { get; set; }
        public string Race     { get; set; }
        public string Class    { get; set; }

        public int Lvl         { get; set; }
        public int Exp         { get; set; }
        public int HpMax       { get; set; }
        public int CurHp       { get; set; }
        public int HpBonus     { get; set; }

        public State Stats     { get; set; }
        public State Modifiers { get; set; }

        public int Gold        { get; set; }
        public List<Item_DnD_5e> Inventory { get; set; } = new List<Item_DnD_5e>();

        public List<State> Debuffs  { get; set; } = new List<State>();
    }
}
