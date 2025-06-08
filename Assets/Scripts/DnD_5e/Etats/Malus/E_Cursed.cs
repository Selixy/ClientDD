using System;

namespace DnD.DnD_5e
{
    public class E_Cursed : Etat_DnD_5e
    {
        public E_Cursed(int             duration   = -1
                       ,DnDRollType?    SType      = null
                       ,int?            dc         = null
                       ,SaveCheckTiming SaveTiming = SaveCheckTiming.None
                       )
                       :base("Maudit"
                            ,"Cette créature souffre d'une malédiction magique. Les effets exacts varient selon la source de la malédiction."
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