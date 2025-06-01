using UnityEngine;


public enum WeaponType { SimpleMelee, SimpleRanged, MartialMelee, MartialRanged }

namespace DnD.DnD_5e
{
    public class Weapon : Item_DnD_5e
    {
        public int DamageDiceCount     { get; set; }
        public int DamageDiceValue     { get; set; }
        public string DamageType       { get; set; }
        public WeaponType Type         { get; set; }
        public bool Finesse            { get; set; }
        public bool TwoHanded          { get; set; }

        public Weapon(string     name
                     ,string     description
                     ,int        weight
                     ,int        value

                     ,int        diceCount
                     ,int        diceValue
                     ,string     damageType
                     ,WeaponType type
                     ,bool       finesse   = false
                     ,bool       twoHanded = false
                     ) 
                     :base(name
                          ,description
                          ,weight
                          ,value
                          )
        {
            DamageDiceCount = diceCount;
            DamageDiceValue = diceValue;
            DamageType = damageType;
            Type = type;
            Finesse = finesse;
            TwoHanded = twoHanded;
        }

        public override void Use(PlayerDND5e user)
        {
            
        }
    }
}
