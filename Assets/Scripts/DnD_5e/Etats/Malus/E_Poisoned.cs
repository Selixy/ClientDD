using System;

namespace DnD.DnD_5e
{
    public class E_Poisoned : Etat_DnD_5e
    {
        public E_Poisoned(int             duration   = -1
                         ,DnDRollType?    SType      = null
                         ,int?            dc         = null
                         ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                         )
                         :base("Empoisonné"
                              ,"La créature a le désavantage à tous ses jets d’attaque et tests de caractéristique."
                              ,EtatType.Malefique
                              ,duration
                              ,SType
                              ,dc
                              ,SaveTiming
                              )
        {
        }
    }
}
