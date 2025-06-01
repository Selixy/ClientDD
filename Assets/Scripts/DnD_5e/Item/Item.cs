using UnityEngine;

namespace DnD.DnD_5e
{
    public abstract class Item_DnD_5e
    {
        public string Name         { get; private set; }
        public string Description  { get; private set; }
        public int    Weight       { get; private set; }
        public int    Value        { get; private set; }

        public Item_DnD_5e(string name
                          ,string description
                          ,int weight
                          ,int value
                          )
        {
            Name = name;
            Description = description;
            Weight = weight;
            Value = value;
        }

        // Méthode commune, mais pouvant être redéfinie
        public abstract void Use(PlayerDND5e user);
    }
}