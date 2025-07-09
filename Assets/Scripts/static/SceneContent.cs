using System.Collections.Generic;

namespace RPG_System
{
    public static class SceneContent
    {
        public static List<Entity> Entities { get; private set; } = new();
        public static List<Item>   Items    { get; private set; } = new();

        static SceneContent() => Load();

        public static void Add(Entity e)    => Entities.Add(e);
        public static void Remove(Entity e) => Entities.Remove(e);
        public static void Add(Item i)      => Items.Add(i);
        public static void Remove(Item i)   => Items.Remove(i);


        public static void Load()
        {

        }

        public static void UnLoad()
        {

        }
    }
}
