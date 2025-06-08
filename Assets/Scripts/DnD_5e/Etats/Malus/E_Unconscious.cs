using System;

namespace DnD.DnD_5e
{
    public class E_Unconscious : Etat_DnD_5e
    {
        public E_Unconscious(int             duration   = -1
                            ,DnDRollType?    SType      = null
                            ,int?            dc         = null
                            ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                            )
                            :base("Inconscient"
                                 ,"La créature est incapable d’agir, inconsciente, lâche tout ce qu’elle tient, et les attaques contre elle ont avantage."
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
