using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class Spell : Skill_DnD_5e
    {
        bool IsRituel;
        bool IsPreapred;

        public Spell(int                   IndexUser
                    ,string                Name          = "[Unknown Spell DnD5e]"
                    ,ActivContext          Context       = ActivContext.NoFight
                    ,int                   Range         = 5
                    ,int                   Radius        = 0
                    ,int?                  DC            = null
                    ,List<DamageComponent> damage        = null
                    ,List<DamageComponent> damageJDS     = null
                    ,List<Etat_DnD_5e>     Etat          = null
                    ,List<Etat_DnD_5e>     EtatJDS       = null
                    ,SkillType             SkillType     = SkillType.Utility
                    ,AttackRoll            AttackRoll    = AttackRoll.None               
                    ,DnDRollType           SauvgardeRoll = DnDRollType.None               
                    )
                    :base(IndexUser
                         ,Name
                         ,Context
                         ,Range
                         ,Radius
                         ,DC
                         ,damage
                         ,damageJDS
                         ,Etat
                         ,EtatJDS
                         ,SkillType
                         ,AttackRoll
                         ,SauvgardeRoll
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