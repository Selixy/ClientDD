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

    public enum SkillType
    {
        None,
        Buff,
        Debuff,
        Summon,
        Utility,
        Attack
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

        public int                   Targetable     {get; protected set;}
        public int                   Range          {get; protected set;}
        public int                   Radius         {get; protected set;}
        public int?                  DC             {get; protected set;}
        public AttackRoll            AttackRoll     {get; protected set;}
        public List<Etat_DnD_5e>     Etats          {get; protected set;}
        public List<Etat_DnD_5e>     EtatsJDS       {get; protected set;}
        public List<DamageComponent> Damages        {get; protected set;}
        public int                   DamagesJDS     {get; protected set;}
        public DnDRollType           SauvegardeRoll {get; protected set;}


        public Skill_DnD_5e(int                   IndexUser
                           ,string                Name          = "[Unknown Skill_DnD_5e]"
                           ,ActivContext          Context       = ActivContext.NoFight
                           ,int                   Targetable    = 0
                           ,int                   Range         = 5
                           ,int                   Radius        = 0
                           ,int?                  DC            = null
                           ,List<DamageComponent> Damages       = null
                           ,int                   DamagesJDS    = 1
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
            this.Targetable     = Targetable;
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
            if ((AttackRoll & AttackRoll.None) == 0)
            {
                var (Success, Crit) = TryAttackRoll(targets);
                if (Crit)
                {
                    damages = GetCriticalDamage(0);
                }
                else
                    targets = Success;
            }

            ApplyToTarget(targets, damages);
            
            base.Cast();
        }
        
        public void ApplyToTarget(List<Entity> targets, List<DamageComponent> damages)
        {
            if (targets == null || targets.Count == 0)
                return;

            if (this.Caster is not Entity_DnD_5e casterDnD)
                return;

            Damage damage = casterDnD.DamageRoll(damages);

            // Filtres et conversion
            var targetDnDList = targets
                .OfType<Entity_DnD_5e>()
                .ToList();

            if (this.SauvegardeRoll != DnDRollType.None)
            {
                var multiJDS = new MultiJDSRequest(targetDnDList, this.SauvegardeRoll, ContextRoll.SauvegardeRoll);
                multiJDS.OnResolved += (results) =>
                {
                    foreach (var target in targetDnDList)
                    {
                        int dc = this.DC ?? 0;
                        int result = results[target];

                        var r_etat = this.Etats;
                        var r_damage = damage;

                        if (result >= dc)
                        {
                            r_etat = this.EtatsJDS;

                            switch (this.DamagesJDS)
                            {
                                default: break;
                                case 1: r_damage = damage / 2; break;
                                case 2: r_damage = new Damage(); break;
                            }
                        }

                        ApplyDamage(target, r_damage);
                        ApplayEtats(target, r_etat);
                    }
                };

                RequestSystem.Enqueue(multiJDS);
            }
            else
            {
                // Pas de JDS → application immédiate
                foreach (var target in targetDnDList)
                {
                    ApplyDamage(target, damage);
                    ApplayEtats(target, this.Etats);
                }
            }
        }

        private void ApplyDamage(Entity target
                                ,Damage damage
                                )
        {
            ((Entity_DnD_5e)target).ApplyDamage(damage);
        }

        private void ApplayEtats(Entity target
                                ,List<Etat_DnD_5e> etats
                                )
        {
            foreach (var e in etats)
            {
                target.AddEtat(e);
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
            if (this.Targetable > 0)
            {
                e = base.GetInRange(this.Caster.Transform.Position, this.Range);
                e = base.ChooseEntity(e, this.Targetable);
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