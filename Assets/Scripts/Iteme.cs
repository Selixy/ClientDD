using System.Collections.Generic;

public class Item
{
    public string Name        { get; protected set; }
    public string Description { get; protected set; }
    public int Weight         { get; protected set; }
    public int Value          { get; protected set; }
    
    public Entity PossessTo   { get; protected set; }


    public Item(string name, string description = null, int weight = 0, int value = 0)
    {
        Name = name;
        Description = description;
        Weight = weight;
        Value = value;
    }

    public virtual void Equip(Entity Entity)
    {
            PossessTo = Entity;
    }
    public virtual void Unequip()
    {
            
    }
}
