using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DnD.DnD_5e
{
    [System.Flags]
    public enum ActivContext
    {
        NoFight       = 1 << 0,
        Action        = 1 << 1,
        bonusAction   = 1 << 2,
        Reaction      = 1 << 3
    }

    [System.Flags]
    public enum SkillType
    {
        Utility       = 1 << 0,

        Melee         = 1 << 1,
        Ranged        = 1 << 2,
        Attack        = 1 << 3,
        AttackRoll    = 1 << 4,

        Buff          = 1 << 5,
        Debuff        = 1 << 6,
        Summon        = 1 << 7
    }

    public enum AttackRoll
    {
        None,
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma,
    }

    public class Skill_DnD_5e : Skill<Entity_DnD_5e>
    {
        public ActivContext          Context    {get; protected set;}
        public SkillType             SkillType  {get; protected set;}

        public int                   Range      {get; protected set;}
        public AttackRoll            AttackRoll {get; protected set;}
        public List<Etat_DnD_5e>     Etat       {get; protected set;}
        public List<DamageComponent> Damage     {get; protected set;}


        public Skill_DnD_5e(int                   IndexUser
                           ,string                Name       = "[Unknown Skill_DnD_5e]"
                           ,ActivContext          Context    = ActivContext.NoFight
                           ,int                   Range      = 5
                           ,List<DamageComponent> damage     = null
                           ,List<Etat_DnD_5e>     Etat       = null
                           ,SkillType             SkillType  = SkillType.Utility
                           ,AttackRoll            AttackRoll = AttackRoll.None               
                           )
                           :base(IndexUser
                                ,Name
                                )
        {
            this.Context    = Context;
            this.Damage     = damage;
            this.AttackRoll = AttackRoll;
        }

        public override void Cast()
        {
            if ((SkillType & SkillType.AttackRoll) != 0)
            {
                // Convertit AttackRoll → DnDRollType
                DnDRollType rollType = AttackRoll switch
                {
                    AttackRoll.Strength     => DnDRollType.Strength,
                    AttackRoll.Dexterity    => DnDRollType.Dexterity,
                    AttackRoll.Constitution => DnDRollType.Constitution,
                    AttackRoll.Intelligence => DnDRollType.Intelligence,
                    AttackRoll.Wisdom       => DnDRollType.Wisdom,
                    AttackRoll.Charisma     => DnDRollType.Charisma,
                    _ => throw new System.Exception("Invalid AttackRoll value")
                };

                var (total, rolls) = Caster.Roll(rollType
                                                ,bonus: 0
                                                ,AddAdvantage: 0
                                                ,Context: ContextRoll.AttackRoll
                                                );
                                                
                int Natural = rolls.FirstOrDefault(r => r.kept).value;
            }

            base.Cast();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}