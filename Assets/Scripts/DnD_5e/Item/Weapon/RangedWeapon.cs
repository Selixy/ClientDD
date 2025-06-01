using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class RangedWeapon : Weapon
    {
        public int RangeMin                  { get; private set; }
        public int RangeMax                  { get; private set; }
        public string AmmunitionType         { get; private set; }
        public List<DamageComponent> RDamage { get; private set; }

        public RangedWeapon(string name          = "[Unknown RangedWeapon]"
                           ,string description   = null
                           ,int weight           = 0
                           ,int value            = 0

                           ,WeaponType           properties  = 0
                           ,int                  bonusAttack = 0
                           ,List<DamageComponent> damage  = null

                           ,List<DamageComponent> rDamage = null
                           ,string  ammunitionType = null
                           ,int     rangeMin       = 6
                           ,int     rangeMax       = 24
                           )
                           :base(name
                                ,description
                                ,weight
                                ,value

                                ,properties
                                ,bonusAttack
                                ,damage
                                )
        {
            this.AmmunitionType = ammunitionType;
            this.RangeMin       = rangeMin;
            this.RangeMax       = rangeMax;

            if (rDamage != null)
                RDamage = rDamage;
            else
                RDamage = damage;

        }
    }
}
