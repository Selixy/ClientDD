using UnityEngine;


namespace DnD.DnD_5e
{
    public class Material : Item_DnD_5e
    {
        public string Usage { get; set; }

        public Material(string name, string description, int weight, int value, string usage)
            : base(name, description, weight, value)
        {
            Usage = usage;
        }

        public override void Use(PlayerDND5e user)
        {
            Debug.Log($"{user.Name} utilise {Name} ({Usage})");
        }
    }
}
