using System;

namespace DnD.DnD_5e
{
    public class E_Frightened : Etat_DnD_5e
    {
        public E_Frightened(int             duration   = -1
                           ,DnDRollType?    SType      = null
                           ,int?            dc         = null
                           ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                           )
                           :base("Effrayé"
                                ,"La créature a le désavantage à ses jets d’attaque et tests de caractéristique tant que la source de sa peur est en vue."
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
