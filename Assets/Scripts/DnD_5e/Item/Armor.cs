using System.Collections.Generic;

public enum ArmorType { Light, Medium, Heavy, Shield }

namespace DnD.DnD_5e
{
    public class Armor : Item_DnD_5e
    {
        public int BonusCA { get; protected set; }
        public bool desavantage {get; protected set;}
        public State requis { get; protected set; }

        public Armor(string name
                    , string description = null
                    , int weight = 0
                    , int value = 0
                    , Harmony HarmonyFlags = 0

                    , int BonusCA = 0
                    , State? requis = null
                    , bool desavantage = false

                    )
                    : base(name
                         , description
                         , weight
                         , value
                         , HarmonyFlags
                         )
        {
            this.BonusCA = BonusCA;
            this.requis = requis ?? new State(0,0,0,0,0,0);
            this.desavantage = desavantage;
        }
    }
}

