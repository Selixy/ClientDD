using System;

namespace DnD.DnD_5e
{
    public class E_ReadiedAction : Etat_DnD_5e
    {
        public E_ReadiedAction(int             duration   = -1
                              ,DnDRollType?    SType      = null
                              ,int?            dc         = null
                              ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                              )
                              :base("Action en attente"
                                   ,"La créature a préparé une action qui se déclenchera en réponse à une condition choisie. L’action est dépensée si la condition survient."
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
