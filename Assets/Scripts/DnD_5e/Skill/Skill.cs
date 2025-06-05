using System.Collections.Generic;
using UnityEngine;

namespace DnD.DnD_5e
{
    [System.Flags]
    public enum ActivContext
    {
        NoFight       = 0,
        Action        = 1 << 0,
        bonusAction   = 1 << 1,
        Reaction      = 1 << 2
    }

    public class Skill_DnD_5e : Skill
    {
        public ActivContext Context;

        public List<DamageComponent> Damage;


        public Skill_DnD_5e(int    IndexUser
                           ,ActivContext Context
                           ,string Name = "[Unknown Skill_DnD_5e]"
                           ,List<DamageComponent> damage  = null
                           )
                           :base(IndexUser
                                ,Name
                                )
        {
            this.Context = Context;
            this.Damage  = damage;

        }
    }
}