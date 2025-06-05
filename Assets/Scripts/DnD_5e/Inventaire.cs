using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class Inventaire_DnD_5e
    {
        public List<Item_DnD_5e> Content { get; private set; }

        public Inventaire_DnD_5e(List<Item_DnD_5e> content = null)
        {
            Content = content ?? new List<Item_DnD_5e>();
        }
    }
}
