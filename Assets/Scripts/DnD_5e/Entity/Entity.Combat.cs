using System.Collections.Generic;
using System.Linq;
using System;

namespace DnD.DnD_5e
{ 
    public partial class Entity_DnD_5e : Entity
    {
        public virtual void StartTurn()
        {
            this.Actions = new int[5]
            {
                (int)(this.WalkSpeed * this.SpeedFactor), // Déplacement disponible ce tour
                1 + this.ExtraAction,                     // Actions principales (ex : Haste = +1 action)
                1,                                        // Bonus action (disponible par défaut)
                1,                                        // Réaction (disponible au départ)
                this.ExtraAtaque                          // Attaques supplémentaires (ex : extra attacks)
            };

            this.fightContext = ActivContext.Action | ActivContext.bonusAction | ActivContext.Reaction;

            foreach (var etat in this.Etats)
            {
                etat.OnTurnStart(this);
            }
        }

        public virtual void EndTurn()
        {
            foreach (int i in new[] { 0, 1, 2, 4 })
                this.Actions[i] = 0;

            this.fightContext &= ~(ActivContext.Action | ActivContext.bonusAction);

            foreach (var etat in this.Etats)
            {
                etat.OnTurnEnd(this);
            }
        }

        public void UseSkillByIndex(int indexType, int indexSkill)
        {
            Skill skillToUse = null;

            switch (indexType)
            {
                case 0: // Full Caster Spell
                    if (indexSkill >= 0 && indexSkill < Spell_FillCaster.Count)
                        skillToUse = Spell_FillCaster[indexSkill];
                    break;

                case 1: // In-Fighter Skill
                    if (indexSkill >= 0 && indexSkill < Skill_InFignter.Count)
                        skillToUse = Skill_InFignter[indexSkill];
                    break;

                case 2: // Half Caster Spell
                    if (indexSkill >= 0 && indexSkill < Spell_HalfCaster.Count)
                        skillToUse = Spell_HalfCaster[indexSkill];
                    break;

                case 3: // Special Casting
                    if (indexSkill >= 0 && indexSkill < Spell_SpecialCaster.Count)
                        skillToUse = Spell_SpecialCaster[indexSkill];
                    break;

                default:
                    return;
            }

            skillToUse?.Cast(this);
        }

        public void UseSkillByName(string name)
        {
            Skill skill = null;

            // Recherche dans tous les groupes
            skill = Spell_FillCaster?.Find(s => s.Name == name)
                ?? Spell_HalfCaster?.Find(s => s.Name == name)
                ?? Spell_SpecialCaster?.Find(s => s.Name == name)
                ?? Skill_InFignter?.Find(s => s.Name == name);

            skill?.Cast(this);
        }
    }
}