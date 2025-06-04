using System.Collections.Generic;

public class Inventaire
{
    public List<Item> Content { get; private set; }

    public Inventaire(List<Item> Content = null)
    {
        this.Content = Content;
    }
}
