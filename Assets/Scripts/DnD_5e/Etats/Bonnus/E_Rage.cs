using System;

namespace DnD.DnD_5e
{
    public class E_Rage : Etat_DnD_5e
    {
        public E_Rage(int             duration   = -1
                     ,DnDRollType?    SType      = null
                     ,int?            dc         = null
                     ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                     )
                     :base("Rage"
                          ,"La créature bénéficie d’avantages aux tests de Force, de bonus aux dégâts de mêlée, et de résistance à certains types de dégâts, mais ne peut pas lancer de sorts."
                          ,EtatType.Benefique
                          ,duration
                          ,SType
                          ,dc
                          ,SaveTiming
                          )
        {
        }
    }
}