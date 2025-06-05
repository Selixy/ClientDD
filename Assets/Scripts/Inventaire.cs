using System.Collections.Generic;

public class Inventaire<T> where T : Item
{
    public List<T> Content { get; private set; }

    public Inventaire(List<T> content = null)
    {
        Content = content ?? new List<T>();
    }
}
