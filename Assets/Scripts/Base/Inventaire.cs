using System.Collections.Generic;

public class Inventaire
{
    public List<Item> Content { get; private set; }

    public Inventaire(List<Item> content = null)
    {
        Content = content ?? new List<Item>();
    }

    public bool TransferTo(Item item, Inventaire targetInventaire)
    {
        if (item == null || targetInventaire == null) return false;
        if (!Content.Contains(item)) return false;

        Content.Remove(item);
        targetInventaire.Content.Add(item);
        return true;
    }

    public void DestructObject(Item item)
    {
        if (!Content.Contains(item))
            return;
        Content.Remove(item);
    }
}