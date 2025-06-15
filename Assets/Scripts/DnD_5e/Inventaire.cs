using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class Inventaire_DnD_5e : Inventaire
    {
        public Inventaire_DnD_5e(List<Item_DnD_5e> content = null)
            : base(CastToBaseList(content)) { }

        private static List<Item> CastToBaseList(List<Item_DnD_5e> content)
        {
            if (content == null) return null;
            List<Item> baseList = new List<Item>(content.Count);
            foreach (var item in content)
                baseList.Add(item); // Implicit cast since Item_DnD_5e : Item
            return baseList;
        }
    }
}
