using UnityEngine;

namespace DnD.DnD_5e
{
    public class Consumable : Item_DnD_5e
    {
        public string Effect { get; set; }

        public Consumable(string name, string description, int weight, int value, string effect)
            : base(name, description, weight, value)
        {
            Effect = effect;
        }

        public override void Use(PlayerDND5e user)
        {
            Debug.Log($"{user.Name} consomme {Name} : effet → {Effect}");
        }
    }
}
