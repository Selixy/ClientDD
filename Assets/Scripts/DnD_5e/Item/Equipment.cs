using UnityEngine;

namespace DnD.DnD_5e
{
    public class Equipment : Item_DnD_5e
    {
        public string Slot { get; set; }

        public Equipment(string name
                        ,string description
                        ,int weight
                        ,int value
                        ,string slot
                        )
                        :base(name
                             ,description
                             ,weight
                             ,value
                             )
        {
            Slot = slot;
        }

        public override void Use(PlayerDND5e user)
        {
            Debug.Log($"{user.Name} équipe {Name} dans l’emplacement {Slot}.");
        }
    }
}

