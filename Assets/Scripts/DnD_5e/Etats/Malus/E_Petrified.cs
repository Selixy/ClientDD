using System;

namespace DnD.DnD_5e
{
    public class E_Petrified : Etat_DnD_5e
    {
        public E_Petrified(int             duration   = -1
                          ,DnDRollType?    SType      = null
                          ,int?            dc         = null
                          ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                          )
                          :base("Pétrifié"
                               ,"La créature est transformée en pierre. Incapable d’agir, insensible à la douleur, immunisée à la plupart des dégâts et états."
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
