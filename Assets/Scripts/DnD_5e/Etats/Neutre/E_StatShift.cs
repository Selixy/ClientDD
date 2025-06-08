using System;

namespace DnD.DnD_5e
{
    public class E_StatShift : Etat_DnD_5e
    {
        public E_StatShift(int             duration   = -1
                          ,DnDRollType?    SType      = null
                          ,int?            dc         = null
                          ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                          )
                          :base("Modification de caractéristique"
                               ,"La créature subit une augmentation ou une diminution temporaire d’une ou plusieurs de ses caractéristiques (FOR, DEX, etc.)"
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
