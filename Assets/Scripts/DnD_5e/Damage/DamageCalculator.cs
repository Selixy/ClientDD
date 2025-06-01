using System.Collections.Generic;
using UnityEngine;

namespace DnD.DnD_5e
{
    public static class DamageCalculator
    {
        public static Damage Calculate(List<DamageComponent> diceList, bool isMagical)
        {
            Damage result = new Damage { Magique = isMagical };

            foreach (var dd in diceList)
            {
                int rolled = dd.Roll();;
                Debug.Log($"Jet de {dd.Dice} → {rolled} ({dd.Type})");

                switch (dd.Type)
                {
                    case DamageType.Contondant:  result.Contondant += rolled; break;
                    case DamageType.Perforant:   result.Perforant  += rolled; break;
                    case DamageType.Tranchant:   result.Tranchant  += rolled; break;
                    case DamageType.Force:       result.Force      += rolled; break;

                    case DamageType.Feu:         result.Feu        += rolled; break;
                    case DamageType.Froid:       result.Froid      += rolled; break;
                    case DamageType.Foudre:      result.Foudre     += rolled; break;
                    case DamageType.Tonnerre:    result.Tonnerre   += rolled; break;

                    case DamageType.Acide:       result.Acide      += rolled; break;
                    case DamageType.Poison:      result.Poison     += rolled; break;
                    
                    case DamageType.Radiant:     result.Radiant    += rolled; break;
                    case DamageType.Nécrotique:  result.Nécrotique += rolled; break;
                    case DamageType.Psychique:   result.Psychique  += rolled; break;
                }
            }

            return result;
        }
    }
}
