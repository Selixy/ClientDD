using System;

namespace DnD.DnD_5e
{
    public class E_Inspiration : Etat_DnD_5e
    {
        public E_Inspiration(int             duration   = -1
                            ,DnDRollType?    SType      = null
                            ,int?            dc         = null
                            ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                            )
                            :base("Inspiration"
                                 ,"La créature peut dépenser l’inspiration pour obtenir l’avantage sur un jet de d20 de son choix."
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
