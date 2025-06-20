using System;
using System.Collections.Generic;
using System.Linq;

namespace DnD.DnD_5e
{
    [System.Flags]
    public enum ActivContext
    {
        NoFight     = 1 << 0,
        Action      = 1 << 1,
        BonusAction = 1 << 2,
        Reaction    = 1 << 3
    }

    public class Skill_DnD_5e : Skill
    {
        // ──────────────── Contexte et type ────────────────
        public ActivContext          Context        { get; protected set; }
        public SkillType             SkillType      { get; protected set; }

        // ──────────────── Portée et cibles ────────────────
        public int                   Targetable     { get; protected set; }
        public int                   Range          { get; protected set; }
        public int                   Radius         { get; protected set; }

        // ──────────────── Jets et conditions ────────────────
        public int?                  DC             { get; protected set; }
        public StateEnum             AttackRoll     { get; protected set; }
        public DnDRollType           SauvegardeRoll { get; protected set; }

        // ──────────────── Effets ────────────────
        public List<Etat_DnD_5e>     Etats          { get; protected set; }
        public List<Etat_DnD_5e>     EtatsJDS       { get; protected set; }

        // ──────────────── Dégâts ────────────────
        public List<DamageComponent> Damages        { get; protected set; }
        public int                   DamagesJDS     { get; protected set; }

        // ──────────────── Constructeur ────────────────
        public Skill_DnD_5e(int                   IndexUser,
                            string                Name          = "[Unknown Skill_DnD_5e]",
                            ActivContext          Context       = ActivContext.NoFight,
                            int                   Targetable    = 0,
                            int                   Range         = 5,
                            int                   Radius        = 0,
                            int?                  DC            = null,
                            List<DamageComponent> Damages       = null,
                            int                   DamagesJDS    = 1,
                            List<Etat_DnD_5e>     Etats         = null,
                            List<Etat_DnD_5e>     EtatsJDS      = null,
                            SkillType             SkillType     = SkillType.Utility,
                            StateEnum             AttackRoll    = StateEnum.None,
                            DnDRollType           SauvgardeRoll = DnDRollType.None)
            : base(IndexUser, Name)
        {
            this.Context        = Context;
            this.Targetable     = Targetable;
            this.Range          = Range;
            this.Radius         = Radius;
            this.DC             = DC;
            this.Damages        = Damages;
            this.DamagesJDS     = DamagesJDS;
            this.Etats          = Etats;
            this.EtatsJDS       = EtatsJDS;
            this.SkillType      = SkillType;
            this.AttackRoll     = AttackRoll;
            this.SauvegardeRoll = SauvgardeRoll;
        }

        // ──────────────── Cast principal ────────────────
        public override void Cast(Entity Caster)
        {
            base.Cast(Caster);

            var targets = SelectEntityHit();
            var damages = this.Damages;

            if ((AttackRoll & StateEnum.None) == 0)
            {
                var (Success, Crit) = TryAttackRoll(targets);
                if (Crit)
                    damages = GetCriticalDamage(0);
                else
                    targets = Success;
            }

            ApplyToTarget(targets, damages);
        }

        // ──────────────── Application des effets ────────────────
        public void ApplyToTarget(List<Entity> targets, List<DamageComponent> damages)
        {
            if (targets == null || targets.Count == 0)
                return;

            if (this.Caster is not Entity_DnD_5e casterDnD)
                return;

            Damage damage = casterDnD.DamageRoll(damages);
            var targetDnDList = targets.OfType<Entity_DnD_5e>().ToList();

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
                            r_damage = this.DamagesJDS switch
                            {
                                1 => damage / 2,
                                2 => new Damage(),
                                _ => damage
                            };
                        }

                        ApplyDamage(target, r_damage);
                        ApplayEtats(target, r_etat);
                    }
                };
                RequestSystem.Enqueue(multiJDS);
            }
            else
            {
                foreach (var target in targetDnDList)
                {
                    ApplyDamage(target, damage);
                    ApplayEtats(target, this.Etats);
                }
            }
        }

        // ──────────────── Utilitaires internes ────────────────
        private void ApplyDamage(Entity target, Damage damage)
        {
            ((Entity_DnD_5e)target).ApplyDamage(damage);
        }

        private void ApplayEtats(Entity target, List<Etat_DnD_5e> etats)
        {
            foreach (var e in etats)
            {
                target.AddEtat(e);
            }
        }

        private List<DamageComponent> GetCriticalDamage(int extraCritDice = 0)
        {
            if (this.Damages == null) return null;

            var result = new List<DamageComponent>();
            foreach (var damage in this.Damages)
            {
                if (damage.Dice.HasValue)
                {
                    Dice critDice = (damage.Dice.Value * 2) + extraCritDice;
                    result.Add(new DamageComponent(critDice, damage.Type));
                }
                else if (damage.Flat.HasValue)
                {
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
            if ((AttackRoll & StateEnum.None) == 0)
            {
                DnDRollType rollType = AttackRoll switch
                {
                    StateEnum.Strength     => DnDRollType.Strength,
                    StateEnum.Dexterity    => DnDRollType.Dexterity,
                    StateEnum.Constitution => DnDRollType.Constitution,
                    StateEnum.Intelligence => DnDRollType.Intelligence,
                    StateEnum.Wisdom       => DnDRollType.Wisdom,
                    StateEnum.Charisma     => DnDRollType.Charisma,
                    _ => throw new Exception("Invalid AttackRoll value")
                };

                var (total, rolls) = ((Entity_DnD_5e)Caster).Roll(rollType, 0, 0, ContextRoll.AttackRoll);
                return (IsPastAC(total, targets), IsCrit(rolls));
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