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
        None          = 0,

        Targetable    = 1 << 1,

        Buff          = 1 << 2,
        Debuff        = 1 << 3,
        Summon        = 1 << 4,
        Utility       = 1 << 5,
        Attack        = 1 << 6
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

    public class Skill_DnD_5e : Skill
    {
        public ActivContext          Context    {get; protected set;}
        public SkillType             SkillType  {get; protected set;}

        public int                   Range      {get; protected set;}
        public int                   Radius     {get; protected set;}
        public AttackRoll            AttackRoll {get; protected set;}
        public List<Etat_DnD_5e>     Etat       {get; protected set;}
        public List<DamageComponent> Damage     {get; protected set;}


        public Skill_DnD_5e(int                   IndexUser
                           ,string                Name       = "[Unknown Skill_DnD_5e]"
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
                                )
        {
            this.Context    = Context;
            this.Damage     = damage;
            this.AttackRoll = AttackRoll;
        }

        public override void Cast()
        {
            var targets = SelectEntityHit();
            var (Success, Crit) = TryAttackRoll(targets);

            base.Cast();
        }

        private List<Entity> SelectEntityHit()
        {
            List<Entity> e = null;
            if ((SkillType & SkillType.Targetable) != 0)
            {
                e = base.GetInRange(this.Caster.Transform.Position, this.Range);
                e = base.ChooseEntity(e);
            } 
            else if (this.Radius < 0)
            {
                e = base.GetInRange(this.Caster.Transform.Position, this.Range);
            }
            return e;
        }

        private (List<Entity> Success, bool Crit) TryAttackRoll(List<Entity> targets)
        {
            if ((AttackRoll & AttackRoll.None) == 0)
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
                var (total, rolls) = ((Entity_DnD_5e)Caster).Roll(rollType
                                                                 ,bonus: 0
                                                                 ,AddAdvantage: 0
                                                                 ,Context: ContextRoll.AttackRoll
                                                                 );                       
                int natural = rolls.FirstOrDefault(r => r.kept).value;
                int result  = total; 


                return (IsPastAC(result, targets), IsCrit(natural));
            }
            return (null, false);

        }

        private bool IsCrit(int natural)
        {
            int seuil = 20 - ((Entity_DnD_5e)this.Caster).CritBonus;
            return natural >= seuil;
        }

        private List<Entity> IsPastAC(int result, List<Entity> targets)
        {
            var hits = new List<Entity>();

            foreach (var entity in targets)
            {
                if (entity is Entity_DnD_5e eDnD && result >= eDnD.AC)
                {
                    hits.Add(eDnD);
                }
            }
            return hits;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}