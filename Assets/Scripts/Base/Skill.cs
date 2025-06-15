using System.Collections.Generic;
using System.Numerics;

public class Skill<TEntity>
{
    public string  Name             { get; protected set; }
    public int     IndexUser        { get; protected set; }
    public bool    IsAvailable      { get; protected set; }
    public bool    ForceUnavailable { get; protected set; }
    public TEntity Caster           { get; protected set; }


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

    public    virtual void Cast()               { }
    public    virtual void DeactivSkil()        { }
    protected virtual void OnSkillUnavailable() { }
    protected virtual void OnSkillAvailable()   { }
    
    private List<TEntity> GetIsHit(Vector3 position, float rayon)
    {
        var hits = new List<TEntity>();

        foreach (var entity in EntityRegistry<TEntity>.All)
        {
            if (entity is Entity<TItem, TEtat> e)
            {
                float distance = Vector3.Distance(e.Transform.Position, position);
                if (distance <= rayon)
                {
                    hits.Add((TEntity)(object)e);
                }
            }
        }

        return hits;
    }
}