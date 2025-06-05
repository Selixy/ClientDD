using System.Collections.Generic;

public class Inventaire<T> where T : Item
{
    public List<T> Content { get; private set; }

    public Inventaire(List<T> content = null)
    {
        Content = content ?? new List<T>();
    }

    public bool TransferTo(T item, Inventaire<T> targetInventaire)
    {
        if (item == null || targetInventaire == null) return false;
        if (!Content.Contains(item)) return false;

        Content.Remove(item);
        targetInventaire.Content.Add(item);
        return true;
    }

    public void DestructObject(T item)
    {
        if (!Content.Contains(item))
            return;
        Content.Remove(item);
    }
}