using UnityEngine;

namespace DnD.DnD_5e
{
    public struct ActivContext
    {
        public bool action;
        public bool bonusAction;
        public bool Reaction;
    }

    public class Skill_DnD_5e
    {
        public ActivContext Context;

        public virtual void Cast()
        {
            Debug.Log("Compétence générique utilisée.");
        }
    }
}
