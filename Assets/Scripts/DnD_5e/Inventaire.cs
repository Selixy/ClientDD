using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class Inventaire_DnD_5e : Inventaire<Item_DnD_5e>
    {

        public Inventaire_DnD_5e(List<Item_DnD_5e> content = null)
                                :base(content)
        {

        }
    }
}
