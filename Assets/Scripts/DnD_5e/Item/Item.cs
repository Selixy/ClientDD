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

    public class Item_DnD_5e
    {
        public string Name             { get; private set; }
        public string Description      { get; private set; }
        public int    Weight           { get; private set; }
        public int    Value            { get; private set; }
        public Entity_DnD_5e PossessTo { get; private set; }
        public Entity_DnD_5e AttunedTo { get; private set; }
        public Harmony HarmonyFlags    { get; private set; }

        public Item_DnD_5e(string name
                          ,string description = null
                          ,int weight = 0
                          ,int value  = 0
                          ,Harmony HarmonyFlags = 0
                          )
        {
            Name = name;
            Description = description;
            Weight = weight;
            Value = value;
        }

        public virtual void Equip(Entity_DnD_5e Entity)
        {
            PossessTo = Entity;
        }
        public virtual void Unequip()
        {
            
        }

        public virtual bool IsAttuned(Entity_DnD_5e entity)
        {
            if (AttunedTo == entity)
                return true;
            else
                return false;
        }

        public virtual bool TryAttune(Entity_DnD_5e entity)
        {
            // Si l'objet ne nécessite PAS d'attunement, on ne fait rien (échec)
            if ((HarmonyFlags & Harmony.RequiresAttunement) == 0)
                return false;

            // Si déjà attuned à une autre entité, refuser
            if (AttunedTo != entity)
                return false;

            // Si déjà attuned, refuser
            if (AttunedTo == entity)
                return false;

            // Sinon, on attune
            AttunedTo = entity;
            HarmonyFlags |= Harmony.IsAttuned;
            return true;
        }

        public virtual bool TryUnattune()
        {
            // Si l'objet est maudit, on ne peut pas l'enlever normalement
            if ((HarmonyFlags & Harmony.Cursed) != 0)
                return false;

            AttunedTo = null;
            HarmonyFlags &= ~Harmony.IsAttuned;
            return true;
        }

    }
}