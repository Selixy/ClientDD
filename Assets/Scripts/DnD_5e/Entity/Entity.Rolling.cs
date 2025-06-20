using System.Collections.Generic;
using System.Linq;
using System;

namespace DnD.DnD_5e
{ 
    public partial class Entity_DnD_5e : Entity
    {
        public (int masteryBonus, int flatBonus) GetSkillBonuses(DnDRollType type)
        {
            if (SkillMastery != null && SkillMastery.TryGetValue(type, out var data))
            {
                int masteryLevel = Math.Clamp(data.MasteryLevel, 0, 2);
                int mastery = this.Maitrise * masteryLevel;
                return (mastery, data.FlatBonus);
            }

            return (0, 0);
        }

        public (int total, int natural) Roll(DnDRollType rollType, int bonus = 0, int AddAdvantage = 0, ContextRoll Context = ContextRoll.Unknown)
        {
            int mod = rollType switch
            {
                // caractéristiques
                DnDRollType.Strength         => Modifiers.Str,
                DnDRollType.Dexterity        => Modifiers.Dex,
                DnDRollType.Constitution     => Modifiers.Con,
                DnDRollType.Intelligence     => Modifiers.Int,
                DnDRollType.Wisdom           => Modifiers.Wis,
                DnDRollType.Charisma         => Modifiers.Cha,

                // jets de sauvegarde
                DnDRollType.StrengthSave     => Modifiers.Str,
                DnDRollType.DexteritySave    => Modifiers.Dex,
                DnDRollType.ConstitutionSave => Modifiers.Con,
                DnDRollType.IntelligenceSave => Modifiers.Int,
                DnDRollType.WisdomSave       => Modifiers.Wis,
                DnDRollType.CharismaSave     => Modifiers.Cha,

                // compétences
                DnDRollType.Athletics        => Modifiers.Str,
                DnDRollType.Acrobatics       => Modifiers.Dex,
                DnDRollType.SleightOfHand    => Modifiers.Dex,
                DnDRollType.Stealth          => Modifiers.Dex,
                DnDRollType.Arcana           => Modifiers.Int,
                DnDRollType.History          => Modifiers.Int,
                DnDRollType.Investigation    => Modifiers.Int,
                DnDRollType.Nature           => Modifiers.Int,
                DnDRollType.Religion         => Modifiers.Int,
                DnDRollType.AnimalHandling   => Modifiers.Wis,
                DnDRollType.Insight          => Modifiers.Wis,
                DnDRollType.Medicine         => Modifiers.Wis,
                DnDRollType.Perception       => Modifiers.Wis,
                DnDRollType.Survival         => Modifiers.Wis,
                DnDRollType.Deception        => Modifiers.Cha,
                DnDRollType.Intimidation     => Modifiers.Cha,
                DnDRollType.Performance      => Modifiers.Cha,
                DnDRollType.Persuasion       => Modifiers.Cha,

                DnDRollType.Initiative       => Modifiers.Dex,

                _ => 0
            };

            var (mastery, flat) = GetSkillBonuses(rollType);
            bonus += mastery + flat;

            var d20 = new Dice(1, 20, DiceType.Neutre);
            var (total, rolls) = d20.Roll(mod + bonus, AddAdvantage);

            var natural = rolls.FirstOrDefault(r => r.kept).value;

            return (total, natural);
        }

        public Damage DamageRoll(List<DamageComponent> diceList, bool isMagical = false)
        {
            Damage result = new Damage { Magique = isMagical };

            foreach (var dd in diceList)
            {
                int rolled = dd.Roll();;

                switch (dd.Type)
                {
                    case DamageType.Contondant:  result.Contondant += rolled; break;
                    case DamageType.Perforant:   result.Perforant  += rolled; break;
                    case DamageType.Tranchant:   result.Tranchant  += rolled; break;
                    case DamageType.Force:       result.Force      += rolled; break;

                    case DamageType.Feu:         result.Feu        += rolled; break;
                    case DamageType.Froid:       result.Froid      += rolled; break;
                    case DamageType.Foudre:      result.Foudre     += rolled; break;
                    case DamageType.Tonnerre:    result.Tonnerre   += rolled; break;

                    case DamageType.Acide:       result.Acide      += rolled; break;
                    case DamageType.Poison:      result.Poison     += rolled; break;

                    case DamageType.Radiant:     result.Radiant    += rolled; break;
                    case DamageType.Nécrotique:  result.Nécrotique += rolled; break;
                    case DamageType.Psychique:   result.Psychique  += rolled; break;
                    case DamageType.Heal:        result.Heal       += rolled; break;
                }
            }
            return result;
        }

        public virtual (int total, int natural) RollJDS(DnDRollType rollType, ContextRoll Context)
        {
            return this.Roll(rollType, Context: Context);
        }

        public virtual int RollInitiative()
        {
            this.Initiative = this.Roll(DnDRollType.Initiative, Context: ContextRoll.initiativeRoll).total;
            return Initiative;
        }
    }
}