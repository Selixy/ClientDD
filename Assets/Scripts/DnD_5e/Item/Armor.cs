using UnityEngine;

public enum ArmorType { Light, Medium, Heavy, Shield }

namespace DnD.DnD_5e
{
    public class Armor : Item_DnD_5e
    {
        public int BaseAC { get; set; }
        public ArmorType Type { get; set; }
        public bool RequiresProficiency { get; set; }

        public Armor(string name
                    ,string description
                    ,int weight
                    ,int value
                    ,int baseAC
                    ,ArmorType type
                    ,bool requiresProficiency = true
                    )
                    :base(name
                         ,description
                         ,weight
                         ,value
                         )
        {
            BaseAC = baseAC;
            Type = type;
            RequiresProficiency = requiresProficiency;
        }

        public override void Use(PlayerDND5e user)
        {
            Debug.Log($"{user.Name} équipe {Name}, Classe d’Armure de base : {BaseAC}");
        }
    }
}

