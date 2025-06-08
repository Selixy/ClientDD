using System;

namespace DnD.DnD_5e
{
    public class E_Grappled : Etat_DnD_5e
    {
        public E_Grappled(int             duration   = -1
                         ,DnDRollType?    SType      = null
                         ,int?            dc         = null
                         ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                         )
                         :base("Attrapé"
                              ,"La vitesse de la créature devient 0 et elle ne peut bénéficier d’aucun bonus à la vitesse. L’état se termine si l’assaillant est incapable ou hors de portée."
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
