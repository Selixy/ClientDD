using System;

namespace DnD.DnD_5e
{
    public class E_Deafened : Etat_DnD_5e
    {
        public E_Deafened(int             duration   = -1
                         ,DnDRollType?    SType      = null
                         ,int?            dc         = null
                         ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                         )
                         :base("Assourdi"
                              ,"La créature ne peut pas entendre et échoue automatiquement à tout test de caractéristique basé sur l’ouïe."
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
