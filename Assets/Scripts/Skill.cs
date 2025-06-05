using UnityEngine;


public class Skill
{
    public string Name        { get; protected set; }
    public bool   IsAvailable { get; protected set; }
    public int    IndexUser   { get; protected set; }

    public Skill(int IndexUser
                ,string Name   = "[Unknown Skill]"
                )
    {
        this.Name = Name;
        this.IndexUser = IndexUser;
    }

    public virtual void Cast()
    {
        Debug.Log("Compétence générique utilisée.");
    }

    public virtual void DeactivSkil()
    {

    }
}

