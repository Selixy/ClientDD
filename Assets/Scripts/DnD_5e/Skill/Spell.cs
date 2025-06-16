using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class Spell : Skill_DnD_5e
    {
        bool IsRituel;
        bool IsPreapred;

        public Spell(int                   IndexUser
                    ,string                Name       = "[Unknown Spell DnD5e]"
                    ,ActivContext          Context    = ActivContext.NoFight
                    ,int                   Range      = 5
                    ,int                   Radius     = 0
                    ,List<DamageComponent> damage     = null
                    ,List<Etat_DnD_5e>     Etat       = null
                    ,SkillType             SkillType  = SkillType.Utility
                    ,AttackRoll            AttackRoll = AttackRoll.None               
                    )
                    :base(IndexUser
                         ,Name
                         ,Context
                         ,Range
                         ,Radius
                         ,damage
                         ,Etat
                         ,SkillType
                         ,AttackRoll
                         )
        {
            
        }

        public override void Update()
        {
            base.ForceUnavailable = !IsPreapred;

            base.Update();
        }
    }
}