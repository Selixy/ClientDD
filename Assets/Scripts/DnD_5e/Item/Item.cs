using UnityEngine;

namespace DnD.DnD_5e
{
    public enum Rarity
    {

    }

    [System.Flags]
    public enum Harmony
    {
        None = 0

    }

    public abstract class Item_DnD_5e
    {
        public string Name         { get; private set; }
        public string Description  { get; private set; }
        public int    Weight       { get; private set; }
        public int    Value        { get; private set; }

        public Item_DnD_5e(string name
                          ,string description = null
                          ,int weight = 0
                          ,int value  = 0
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