using System;

namespace DnD.DnD_5e
{
    public abstract class Etat_DnD_5e : Etat<Item_DnD_5e, Entity_DnD_5e>
    {
        public int? DC_Sauvegarde { get; protected set; }

        protected Etat_DnD_5e(string    nom          =  "[Unknown Etat DnD 5e]"
                             ,string    description  =  null
                             ,EtatType  type         =  0
                             ,int       duree        =  -1
                             ,int?      dc           =  null
                             )
                             :base(nom
                                  ,description
                                  ,type
                                  ,duree
                                  )
        {
            DC_Sauvegarde = dc;
        }

        public virtual bool TrySave(Entity_DnD_5e cible, int jet)
        {
            return DC_Sauvegarde.HasValue && jet >= DC_Sauvegarde.Value;
        }
    }
}