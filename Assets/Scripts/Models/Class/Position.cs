using System.Numerics;

namespace RPG_System
{
    public class Position
    {
        public Vector3?   Vector  {get; private set;} = null;
        public Entity     Entity  {get; private set;} = null;
        public Item       Item    {get; private set;} = null;

        public Position(Vector3 vector) => this.Vector = vector;
        public Position(Entity entity)  => this.Entity = entity;
        public Position(Item item)      => this.Item   = item;

        public Vector3 GetPosition()
        {
            if (this.Vector.HasValue) return this.Vector.Value;
            if (this.Entity != null)  return this.Entity.Position;
            if (this.Item != null)    return this.Item.Position;
            return Vector3.Zero;
        }

        public bool IsValid =>
            this.Vector.HasValue || this.Entity != null || this.Item != null;

        public bool IsPosses =>
            this.Entity != null || this.Item != null;


        // Retourne la source actuelle de la position
        public object GetSource()
        {
            if (this.Vector.HasValue) return this.Vector.Value;
            if (this.Entity != null)  return this.Entity;
            if (this.Item != null)    return this.Item;
            return null;
        }

        // Operator
        public static explicit operator Vector3(Position p) => p.GetPosition();
        
        public static implicit operator Position(Vector3 v) => new Position(v);
        public static implicit operator Position(Entity e)  => new Position(e);
        public static implicit operator Position(Item i)    => new Position(i);
    }
}