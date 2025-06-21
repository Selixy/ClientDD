namespace DnD.DnD_5e
{
    public enum SpellcastingStyle
    {
        None,          // Pas de magie (Barbare)
        Known,         // Liste fixe de sorts connus (Sorcerer, Bard, Warlock, Ranger)
        Prepared,      // Préparés à partir d’une liste fixe (Cleric, Druid, Paladin)
        Grimoire       // Préparés depuis un grimoire (Wizard)
    }
}