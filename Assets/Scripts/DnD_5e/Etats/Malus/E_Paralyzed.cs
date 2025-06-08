using System;

namespace DnD.DnD_5e
{
    public class E_Paralyzed : Etat_DnD_5e
    {
        public E_Paralyzed(int             duration   = -1
                          ,DnDRollType?    SType      = null
                          ,int?            dc         = null
                          ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                          )
                          :base("Paralysé"
                               ,"La créature est incapable de bouger, échoue automatiquement aux sauvegardes de Force et de Dextérité, et les attaques contre elle ont avantage."
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
