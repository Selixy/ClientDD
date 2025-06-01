using System.Collections.Generic;
using UnityEngine;

namespace DnD.DnD_5e
{
    public class Attack : Skill_DnD_5e
    {
        public bool IsMagical = false;
        public List<DamageComponent> Damage = new List<DamageComponent>();

        public override void Cast()
        {
            // Comportement de compétence générique
            base.Cast();

            // Calcul des dégâts
            Damage result = DamageCalculator.Calculate(Damage, IsMagical);

            // Affichage
            Debug.Log(result);
        }
    }
}
