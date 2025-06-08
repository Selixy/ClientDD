using System;

namespace DnD.DnD_5e
{
    public class E_Hasted : Etat_DnD_5e
    {
        public E_Hasted(int             duration   = -1
                       ,DnDRollType?    SType      = null
                       ,int?            dc         = null
                       ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                       )
                       :base("Accéléré"
                            ,"La créature voit sa vitesse doublée, gagne un bonus de +2 à la CA, a l’avantage aux jets de Dextérité, et bénéficie d’une action supplémentaire à chaque tour."
                            ,EtatType.Benefique
                            ,duration
                            ,SType
                            ,dc
                            ,SaveTiming
                            )
        {
        }
    }
}
