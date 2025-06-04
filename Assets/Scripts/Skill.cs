using UnityEngine;


public class Skill
{
    public bool IsAvailable {get; protected set;}
    public int  IndexUser   {get; protected set;}

    public virtual void Cast()
    {
        Debug.Log("Compétence générique utilisée.");
    }

    public virtual void DeactivSkil()
    {

    }
}

