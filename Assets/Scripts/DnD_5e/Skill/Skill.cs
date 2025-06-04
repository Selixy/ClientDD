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

        public Skill_DnD_5e(ActivContext Context)
        {
            this.Context = Context;
        }
    }
}