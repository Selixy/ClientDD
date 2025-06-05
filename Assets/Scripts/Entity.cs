using System.Collections.Generic;

public class Entity
{
    public string           Name       { get; protected set; }
    public int              Lvl        { get; protected set; }
    public int              Exp        { get; protected set; }
    public int              HpMax      { get; protected set; }
    public int              CurHp      { get; protected set; }
    public int              HpBonus    { get; protected set; }
    public Inventaire<Item> Inventaire { get; protected set; }

    public virtual void TakeDamage(int amount)
    {
        CurHp -= amount;
        if (CurHp < 0) CurHp = 0;
    }

    public virtual void Heal(int amount)
    {
        CurHp += amount;
        if (CurHp > HpMax + HpBonus) CurHp = HpMax + HpBonus;
    }

    public Entity(string name = "[Unknown Entity]"
                 ,int lvl     = 1
                 ,int exp     = 0
                 ,int hpMax   = 1
                 )
    {
        Name  = name;
        Lvl   = lvl;
        Exp   = exp;
        HpMax = hpMax;
        CurHp = hpMax;
    }
}
