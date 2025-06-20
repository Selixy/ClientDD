using System.Collections.Generic;
using System.Numerics;

public class Skill
{
    public string  Name             { get; protected set; }
    public int     IndexUser        { get; protected set; }
    public bool    IsAvailable      { get; protected set; }
    public bool    ForceUnavailable { get; protected set; }
    public Entity  Caster           { get; protected set; }


    private bool lastAviability = false;

    public Skill(int IndexUser
                ,string Name   = "[Unknown Skill]"
                )
    {
        this.Name = Name;
        this.IndexUser = IndexUser;
    }

    public virtual void Update()
    {
        if (ForceUnavailable) IsAvailable = false;

        if (lastAviability != IsAvailable)
        {
            if (IsAvailable)
            {
                OnSkillUnavailable();
            }
            else
            {
                OnSkillAvailable();
            }
            lastAviability = IsAvailable;
        }
    }

    public    virtual void Cast( Entity Caster)
    {
        this.Caster = Caster;
    }
    
    public    virtual void DeactivSkil()           { }
    protected virtual void OnSkillUnavailable()    { }
    protected virtual void OnSkillAvailable()      { }
    

    public    virtual void CastToEntity(Entity e) { }
    
    public    virtual List<Entity> ChooseEntity(List<Entity> e, int n)
    {
        return null;
    }

    protected List<Entity> GetInRange(Vector3 position, float rayon)
    {
        var hits = new List<Entity>();

        foreach (var entity in EntityRegistry.All)
        {
            if (entity is Entity e)
            {
                float distance = Vector3.Distance(e.Transform.Position, position);
                if (distance <= rayon)
                {
                    hits.Add((Entity)(object)e);
                }
            }
        }

        return hits;
    }
}