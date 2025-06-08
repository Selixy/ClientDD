using System;

namespace DnD.DnD_5e
{
    public class E_Invisible : Etat_DnD_5e
    {
        public E_Invisible(int             duration   = -1
                          ,DnDRollType?    SType      = null
                          ,int?            dc         = null
                          ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                          )
                          :base("Invisible"
                               ,"La créature ne peut pas être vue sans magie ou sens spécial. Les attaques contre elle ont désavantage, et elle a l’avantage à ses propres attaques."
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
