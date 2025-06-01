using UnityEngine;

namespace DnD.DnD_5e
{
    public class Equipment : Item_DnD_5e
    {

        public Equipment(string name
                        ,string description = null
                        ,int weight = 0
                        ,int value  = 0
                        ,Harmony HarmonyFlags = 0
                        )
                        :base(name
                             ,description
                             ,weight
                             ,value
                             ,HarmonyFlags
                             )
        {

        }

    }
}

