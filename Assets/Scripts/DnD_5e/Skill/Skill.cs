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
        public ActivContext          Context        {get; protected set;}
        public SkillType             SkillType      {get; protected set;}

        public int                   Range          {get; protected set;}
        public int                   Radius         {get; protected set;}
        public int?                  DC             {get; protected set;}
        public AttackRoll            AttackRoll     {get; protected set;}
        public List<Etat_DnD_5e>     Etats          {get; protected set;}
        public List<Etat_DnD_5e>     EtatsJDS       {get; protected set;}
        public List<DamageComponent> Damages        {get; protected set;}
        public List<DamageComponent> DamagesJDS     {get; protected set;}
        public DnDRollType           SauvegardeRoll {get; protected set;}


        public Skill_DnD_5e(int                   IndexUser
                           ,string                Name          = "[Unknown Skill_DnD_5e]"
                           ,ActivContext          Context       = ActivContext.NoFight
                           ,int                   Range         = 5
                           ,int                   Radius        = 0
                           ,int?                  DC            = null
                           ,List<DamageComponent> Damages       = null
                           ,List<DamageComponent> DamagesJDS    = null
                           ,List<Etat_DnD_5e>     Etats         = null
                           ,List<Etat_DnD_5e>     EtatsJDS      = null
                           ,SkillType             SkillType     = SkillType.Utility
                           ,AttackRoll            AttackRoll    = AttackRoll.None               
                           ,DnDRollType           SauvgardeRoll = DnDRollType.None               
                           )
                           :base(IndexUser
                                ,Name
                                )
        {
            this.Context        = Context;
            this.Damages        = Damages;
            this.DamagesJDS     = DamagesJDS;
            this.Etats          = Etats;
            this.EtatsJDS       = EtatsJDS;
            this.AttackRoll     = AttackRoll;
            this.SauvegardeRoll = SauvegardeRoll;
        }

        public override void Cast()
        {
            var targets = SelectEntityHit();

            List<DamageComponent> damages = this.Damages;
            List<DamageComponent> damagesJDS = this.DamagesJDS;
            if ((AttackRoll & AttackRoll.None) == 0)
            {
                var (Success, Crit) = TryAttackRoll(targets);
                if (Crit)
                {
                    damages = GetCriticalDamage(0);
                    damagesJDS = GetCriticalDamage(0);
                }
                else
                    targets = Success;
            }

            ApplyToTarget(targets, damages, damagesJDS);
            
            base.Cast();
        }

        public void ApplyToTarget(List<Entity> targets
                                 ,List<DamageComponent> damages
                                 ,List<DamageComponent> damagesJDS
                                 )
        {
            foreach (var target in targets)
            {
                if (targets == null) return;

                var etats  = this.Etats;
                int dc = this.DC != null ? this.DC.Value : 0;

                var r_damage = damagesJDS;
                var r_etat   = this.EtatsJDS;

                if (this.SauvegardeRoll != DnDRollType.None)
                {
                    var result = ((Entity_DnD_5e)target).RequestJDS(this.SauvegardeRoll
                                                                   ,ContextRoll.SauvegardeRoll
                                                                   );
                    if (result.total <= dc)
                    {
                        r_damage = damages;
                        r_etat   = this.Etats;
                    }
                    ApplayDamage(target, r_damage);
                    ApplayEtats(target, r_etat);
                }

            }
        }

        private void ApplayDamage(Entity targets
                                 ,List<DamageComponent> damages
                                 )
        {
            
        }

        private void ApplayEtats(Entity targets
                                ,List<Etat_DnD_5e> etats
                                )
        {
            List<Etat> etatsBase = new List<Etat>();
            foreach (var e in etats)
            {
                etatsBase.Add(e);
            }
        }

        public List<DamageComponent> GetCriticalDamage(int extraCritDice = 0)
        {
            var result = new List<DamageComponent>();
            
            if (this.Damages == null) return null;

            foreach (var damage in this.Damages)
            {
                if (damage.Dice.HasValue)
                {
                    // On double le nombre de dés, puis ajoute les dés supplémentaires éventuels
                    Dice critDice = (damage.Dice.Value * 2) + extraCritDice;
                    result.Add(new DamageComponent(critDice, damage.Type));
                }
                else if (damage.Flat.HasValue)
                {
                    // Dégâts fixes (pas modifiés par un critique normalement)
                    result.Add(new DamageComponent(damage.Flat.Value, damage.Type));
                }
            }

            return result;
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
                int natural = rolls;
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