using System.Collections.Generic;
using UnityEngine;

namespace DnD.DnD_5e
{
    [System.Flags]
    public enum WeaponType
    {
        None        = 0,        // Aucune propriété
        Light       = 1 << 0,   // Légère : peut être utilisée pour une attaque à deux armes sans malus
        Finesse     = 1 << 1,   // Finesse : peut utiliser la Dextérité au lieu de la Force pour l’attaque et les dégâts
        Thrown      = 1 << 2,   // Lancer : peut être lancée à distance
        Versatile   = 1 << 3,   // Polyvalente : peut être utilisée à une ou deux mains (les dégâts changent)
        TwoHanded   = 1 << 4,   // Deux mains : nécessite les deux mains pour attaquer
        Heavy       = 1 << 5,   // Lourde : donne un désavantage aux petites créatures
        Reach       = 1 << 6,   // Allonge : portée de mêlée augmentée à 3m (au lieu de 1.5m)
        Ammunition  = 1 << 7,   // Munitions : tire un projectile et consomme une munition
        Loading     = 1 << 8,   // Rechargement : une seule attaque possible par action même avec Extra Attack
        Special     = 1 << 9    // Spéciale : nécessite une règle unique spécifique (ex: filet)
    }

    public class Weapon : Item_DnD_5e
    {
        public WeaponType Properties         { get; protected set; }
        public List<DamageComponent> Damage  { get; protected set; }
        public int BonusAttack               { get; protected set; }

        public Weapon(string name          = "[Unknown Weapon]"
                     ,string description   = null
                     ,int    weight        = 0
                     ,int    value         = 0
                     ,Harmony HarmonyFlags = 0

                     ,WeaponType           properties  = 0
                     ,int                  bonusAttack = 0
                     ,List<DamageComponent> damage  = null
                     )
                     :base(name
                          ,description
                          ,weight
                          ,value
                          ,HarmonyFlags
                          )
        {
            Properties  = properties;
            BonusAttack = bonusAttack;
            Damage      = damage;
            

        }


    }
}
