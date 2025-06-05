using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class Spell : Skill_DnD_5e
    {
        bool IsRituel;

        public Spell(int IndexUser
                    ,string Name
                    ,ActivContext Context
                    ,List<DamageComponent> damage  = null
                    )
                    :base(IndexUser
                         ,Context
                         ,Name
                         ,damage
                         )
        {

        }
    }
}