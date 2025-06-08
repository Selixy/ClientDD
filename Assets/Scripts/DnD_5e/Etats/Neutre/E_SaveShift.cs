using System;

namespace DnD.DnD_5e
{
    public class E_SaveShift : Etat_DnD_5e
    {
        public E_SaveShift(int             duration   = -1
                          ,DnDRollType?    SType      = null
                          ,int?            dc         = null
                          ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                          )
                          :base("Modificateur de sauvegarde"
                               ,"La créature reçoit un bonus ou un malus à ses jets de sauvegarde spécifiés pendant la durée de l’effet."
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
