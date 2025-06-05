using System.Collections.Generic;
using UnityEngine;

namespace DnD.DnD_5e
{
    public enum Types
    {
        potion = 0,
        munition = 1,
        ration = 2,  
        material =3,      
    }
    
    public class Consumable : Item_DnD_5e
    {
        public string effect { get; protected set; }
        public Types types {get; protected set;}
        public List<DamageComponent> damage { get; protected set; }

        public Consumable(string name
                         , string description = null
                         , int weight = 0
                         , int value = 0
                         , Harmony HarmonyFlags = 0

                         , Types type = 0
                         , string effect = null
                         , List<DamageComponent> damage = null

                         )
                         : base(name
                              , description
                              , weight
                              , value
                              , HarmonyFlags
                              )
        {
            this.types = type;
            this.damage = damage;
            this.effect = effect;
        }

    }
}
