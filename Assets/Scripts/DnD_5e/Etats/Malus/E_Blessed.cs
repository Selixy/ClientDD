using System;

namespace DnD.DnD_5e
{
    public class E_Blinded : Etat_DnD_5e
    {
        public E_Blinded(int             duration   = -1
                        ,DnDRollType?    SType      = null
                        ,int?            dc         = null
                        ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                        )
                        :base("Aveugle"
                             ,"La créature ne peut pas voir et échoue automatiquement à tout test de caractéristique nécessitant la vue. " +
                              "Les jets d’attaque contre la créature ont l’avantage, et les jets d’attaque de la créature ont le désavantage."
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
