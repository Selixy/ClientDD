using System.Collections.Generic;

public enum EtatType
{
    Benefique,
    Neutre,
    Malefique
}

public class Etat
{
    public string    Nom         { get; protected set; }
    public string    Description { get; protected set; }
    public EtatType  Type        { get; protected set; }
    public int       Duree       { get; protected set; } // Nombre de tours, -1 = permanent
    public bool      EstVisible  { get; protected set; } = true;
    public Entity   Owner       { get; protected set; } 


    public Etat(string nom
               ,string description
               ,EtatType type
               ,int duree = -1
               )
    {
        Nom         = nom;
        Description = description;
        Type        = type;
        Duree       = duree;
    }

    public virtual void OnTurnStart(Entity entite) { }
    public virtual void OnTurnEnd(Entity entite)   { }

    public virtual void OnRemove()    
    { 
        ReflectMethod(this.Owner, "RemoveEtat", this);
        this.Owner = null;
    }

    public virtual void OnAply(Entity entity)      
    { 
        ReflectMethod(entity, "AddEtat", this);
        this.Owner = entity;
    }

    public virtual void Trasfert(Entity newEntity)
    {
        OnRemove();
        OnAply(newEntity);
    }

    void ReflectMethod(object o, string methodName, params object[] args)
    {
        if (o == null || string.IsNullOrEmpty(methodName))
            return;

        var method = o.GetType().GetMethod(methodName);
        if (method != null)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == args.Length)
                method.Invoke(o, args);
            else if (parameters.Length == 0)
                method.Invoke(o, null);
        }
    }
}