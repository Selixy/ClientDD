using System;

namespace DnD.DnD_5e
{
    public class E_Charmed : Etat_DnD_5e
    {
        public E_Charmed(int             duration   = -1
                        ,DnDRollType?    SType      = null
                        ,int?            dc         = null
                        ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                        )
                        :base("Charmé"
                             ,"Une créature charmée ne peut pas attaquer l’enchanteur ni lui faire de mal par des effets magiques. " +
                              "L’enchanteur a l’avantage à tous les tests de Charisme pour interagir socialement avec la créature charmée."
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