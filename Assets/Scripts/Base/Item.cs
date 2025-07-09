using System.Numerics;

namespace RPG_System
{
    public class Item
    {
        public Position Position { get; private set; }

        public Item(Vector3? position = null)
        {
            this.Position = new Position(position ?? Vector3.Zero);
        }
    }
}