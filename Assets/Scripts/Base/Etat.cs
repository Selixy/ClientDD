using System.Collections.Generic;

public enum EtatType
{
    Benefique,
    Neutre,
    Malefique
}

public class Etat<TItem, TEntity>
{
    public string    Nom         { get; set; }
    public string    Description { get; set; }
    public EtatType  Type        { get; set; }
    public int       Duree       { get; set; } // Nombre de tours, -1 = permanent
    public bool      EstVisible  { get; set; } = true;

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

    public virtual void OnTurnStart(TEntity entite) { }
    public virtual void OnTurnEnd(TEntity entite)   { }
    public virtual void OnRemove(TEntity entite)    { }
}