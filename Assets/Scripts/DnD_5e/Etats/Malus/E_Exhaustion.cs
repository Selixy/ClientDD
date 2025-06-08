using System;

namespace DnD.DnD_5e
{
    public class E_Exhaustion : Etat_DnD_5e
    {
        public E_Exhaustion(int             duration   = -1
                           ,DnDRollType?    SType      = null
                           ,int?            dc         = null
                           ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                           )
                           :base("Épuisement"
                                ,"L’épuisement a plusieurs niveaux cumulables, chacun ayant ses propres effets néfastes."
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
