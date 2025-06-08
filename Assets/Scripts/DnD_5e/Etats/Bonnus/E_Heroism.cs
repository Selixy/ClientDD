using System;

namespace DnD.DnD_5e
{
    public class E_Heroism : Etat_DnD_5e
    {
        public E_Heroism(int             duration   = -1
                        ,DnDRollType?    SType      = null
                        ,int?            dc         = null
                        ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                        )
                        :base("Héroïsme"
                             ,"La créature est immunisée à la peur et gagne des points de vie temporaires à chaque début de tour."
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
