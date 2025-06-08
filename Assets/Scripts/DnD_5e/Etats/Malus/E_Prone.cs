using System;

namespace DnD.DnD_5e
{
    public class E_Prone : Etat_DnD_5e
    {
        public E_Prone(int             duration   = -1
                      ,DnDRollType?    SType      = null
                      ,int?            dc         = null
                      ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                      )
                      :base("À terre"
                           ,"La créature ne peut avancer qu’en rampant. Les attaques au corps à corps contre elle ont avantage, celles à distance ont désavantage."
                           ,EtatType.Neutre
                           ,duration
                           ,SType
                           ,dc
                           ,SaveTiming
                           )
        {
        }
    }
}
