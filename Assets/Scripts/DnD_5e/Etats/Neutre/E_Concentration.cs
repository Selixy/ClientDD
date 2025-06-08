using System;

namespace DnD.DnD_5e
{
    public class E_Concentration : Etat_DnD_5e
    {
        public E_Concentration(int             duration   = -1
                              ,DnDRollType?    SType      = null
                              ,int?            dc         = null
                              ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                              )
                              :base("Concentration"
                                   ,"La créature maintient sa concentration sur un sort. Elle doit réussir un jet de sauvegarde de Constitution pour ne pas perdre la concentration en cas de dégâts ou de perturbation."
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
