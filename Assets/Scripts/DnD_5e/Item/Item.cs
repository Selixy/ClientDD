using UnityEngine;

namespace DnD.DnD_5e
{
    public enum Rarity
    {
        Common,         // Commun
        Uncommon,       // Peu commun
        Rare,           // Rare
        VeryRare,       // Très rare
        Legendary,      // Légendaire
        Artifact,       // Artefact (objets uniques et très puissants)
        Unknown         // Inconnu (optionnel, utile pour des objets mystérieux)
    }

    [System.Flags]
    public enum Harmony
    {
        None               = 0,             // Objet non magique ou sans effet particulier
        Magical            = 1 << 0,        // Objet possède une propriété magique
        RequiresAttunement = 1 << 1,        // Nécessite une harmonisation
        IsAttuned          = 1 << 2,        // Est actuellement harmonisé avec un porteur
        Cursed             = 1 << 3,        // Objet maudit (ne peut pas être déséquipé facilement)
        Sentient           = 1 << 4,        // Objet doté d’une conscience (volonté propre)
        Unique             = 0 << 5         // Objet unique dans le monde
    }

    public abstract class Item_DnD_5e
    {
        public string Name             { get; private set; }
        public string Description      { get; private set; }
        public int    Weight           { get; private set; }
        public int    Value            { get; private set; }
        public Entity_DnD_5e AttunedTo { get; private set; }

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

    }
}