using System.Collections.Generic;

public class Inventaire<TItem>
{
    public List<TItem> Content { get; private set; }

    public Inventaire(List<TItem> content = null)
    {
        Content = content ?? new List<TItem>();
    }

    public bool TransferTo(TItem item, Inventaire<TItem> targetInventaire)
    {
        if (item == null || targetInventaire == null) return false;
        if (!Content.Contains(item)) return false;

        Content.Remove(item);
        targetInventaire.Content.Add(item);
        return true;
    }

    public void DestructObject(TItem item)
    {
        if (!Content.Contains(item))
            return;
        Content.Remove(item);
    }
}