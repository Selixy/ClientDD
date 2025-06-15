using System;

namespace DnD.DnD_5e
{
    public enum SaveCheckTiming
    {
        None,            // Pas de save (état persistant)
        StartOfTurn,     // Sauvegarde au début de chaque tour
        EndOfTurn,       // Sauvegarde à la fin de chaque tour
        OnAction,        // Sauvegarde seulement sur une action spécifique
        Custom           // Si besoin d’un cas particulier
    }

    public abstract class Etat_DnD_5e : Etat
    {
        public SaveCheckTiming? SaveTiming     { get; protected set; }
        public int?             DC_Sauvegarde  { get; protected set; }
        public DnDRollType?     SType          { get; protected set; }


        protected Etat_DnD_5e(string        nom           = "[Unknown Etat DnD 5e]"
                             ,string        description   = null
                             ,EtatType      type          = 0
                             ,int           duree         = -1
                             ,DnDRollType?  SType         = null
                             ,int?          dc            = null
                             ,SaveCheckTiming? SaveTiming = SaveCheckTiming.None
                             )
                             :base(nom
                                  ,description
                                  ,type
                                  ,duree
                                  )
        {
            this.DC_Sauvegarde = dc;
            this.SType         = SType;
        }

        public override void OnTurnStart(Entity entity)
        {
            if (entity is not Entity_DnD_5e) return;
            if (SaveTiming == SaveCheckTiming.StartOfTurn)
            {
                TryRemoveWithSave((Entity_DnD_5e)entity);
            }

            base.OnTurnStart(entity);
        }

        public override void OnTurnEnd(Entity entity)
        {
            if (entity is not Entity_DnD_5e) return;
            if (SaveTiming == SaveCheckTiming.EndOfTurn)
            {
                TryRemoveWithSave((Entity_DnD_5e)entity);
            }

            base.OnTurnEnd(entity);
        }

        protected virtual void TryRemoveWithSave(Entity_DnD_5e entity)
        {
            if (!DC_Sauvegarde.HasValue || this.SType == null) return;

            int jet = entity.Roll(this.SType.Value).total;
            if (TrySave(entity, jet))
            {
                entity.RemoveEtat(this);
            }
        }

        public virtual bool TrySave(Entity_DnD_5e cible, int jet)
        {
            return DC_Sauvegarde.HasValue && jet >= DC_Sauvegarde.Value;
        }
        
        public override void OnRemove()
        {
            base.OnRemove();
        }

        public override void OnAply(Entity entity)
        {
            base.OnAply(entity);
        }
    }
}