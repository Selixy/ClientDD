using UnityEngine;

namespace DnD.DnD_5e
{
    public class Consumable : Item_DnD_5e
    {
        public string Effect { get; set; }

        public enum Types
        {
            potion              = 0,             
            munition            = 1,        
            ration              = 2,        
        }
        

        public Consumable(string name
                         , string description = null
                         , int weight = 0
                         , int value = 0
                         , Harmony HarmonyFlags = 0

                         , int nombre = 0
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
            this.nombre = nombre;
            this.damage = damage;
            this.effect = effect;
        }

    }
}
