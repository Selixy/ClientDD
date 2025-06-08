using System;

namespace DnD.DnD_5e
{
    public class E_Restrained : Etat_DnD_5e
    {
        public E_Restrained(int             duration   = -1
                           ,DnDRollType?    SType      = null
                           ,int?            dc         = null
                           ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                           )
                           :base("Entravé"
                                ,"La vitesse de la créature devient 0, elle a le désavantage à ses jets d’attaque et les attaques contre elle ont avantage."
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
