using UnityEngine;

namespace DnD.DnD_5e
{
    public class Accessoires : Item_DnD_5e
    {

        public Accessoires (string name
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

