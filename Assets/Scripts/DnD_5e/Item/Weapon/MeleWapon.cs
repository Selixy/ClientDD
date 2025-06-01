using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class MeleeWeapon : Weapon
    {
        public float Reach                   { get; private set; }
        public List<DamageComponent> VDamage { get; protected set; }

        public MeleeWeapon(string name          = "[Unknown MeleeWeapon]"
                          ,string description   = null
                          ,int weight           = 0
                          ,int value            = 0

                          ,WeaponType           properties  = 0
                          ,int                  bonusAttack = 0
                          ,List<DamageComponent> damage  = null

                          ,List<DamageComponent> vDamage = null
                          ,float reach = 1.5f
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
            this.Reach = reach;

            if (vDamage != null && properties.HasFlag(WeaponType.Versatile))
                VDamage = vDamage;
            else
                VDamage = damage;
        }
    }
}
