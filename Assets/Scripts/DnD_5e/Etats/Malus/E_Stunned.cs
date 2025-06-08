using System;

namespace DnD.DnD_5e
{
    public class E_Stunned : Etat_DnD_5e
    {
        public E_Stunned(int             duration   = -1
                        ,DnDRollType?    SType      = null
                        ,int?            dc         = null
                        ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                        )
                        :base("Hébété"
                             ,"La créature est incapable d’agir, ne peut ni bouger ni parler, et échoue automatiquement à certains jets de sauvegarde."
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
