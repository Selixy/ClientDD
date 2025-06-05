using UnityEngine;
using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class Accessoires : Item_DnD_5e
    {
        public int bonus { get; protected set; }
        public List<DamageComponent> damage { get; protected set; }
        public Accessoires(string name
                        , string description = null
                        , int weight = 0
                        , int value = 0
                        , Harmony HarmonyFlags = 0

                        , int Bonus = 0
                        , List<DamageComponent> damage = null

                        )
                        :base(name
                             ,description
                             ,weight
                             ,value
                             ,HarmonyFlags
                             )
        {
            this.bonus = bonus;
            this.damage = damage;
            
        }

    }
}

