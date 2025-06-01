using UnityEngine;

public enum ArmorType { Light, Medium, Heavy, Shield }

namespace DnD.DnD_5e
{
    public class Armor : Item_DnD_5e
    {
        public Armor(string name
                    ,string description = null
                    ,int weight = 0
                    ,int value  = 0
                    )
                    :base(name
                         ,description
                         ,weight
                         ,value
                         )
        {
            
        }
    }
}

