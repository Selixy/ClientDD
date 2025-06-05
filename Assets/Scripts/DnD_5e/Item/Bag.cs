using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class Bag : Item_DnD_5e
    {
        public Inventaire_DnD_5e Content { get; private set; }

        public Bag(string name
                  ,string description = null
                  ,int weight = 0
                  ,int value  = 0
                  ,Harmony HarmonyFlags = 0
                  ,List<Item_DnD_5e> content = null
                  )
                  :base(name
                       ,description
                       ,weight
                       ,value
                       ,HarmonyFlags
                       )
        {
            this.Content = new Inventaire_DnD_5e(content);
        }
    }
}