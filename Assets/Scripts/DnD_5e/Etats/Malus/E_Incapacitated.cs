using System;

namespace DnD.DnD_5e
{
    public class E_Incapacitated : Etat_DnD_5e
    {
        public E_Incapacitated(int             duration   = -1
                              ,DnDRollType?    SType      = null
                              ,int?            dc         = null
                              ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                              )
                              :base("Incapable d’agir"
                                   ,"La créature ne peut pas entreprendre d’actions ni de réactions."
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
