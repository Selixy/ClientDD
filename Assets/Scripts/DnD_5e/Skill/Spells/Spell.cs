namespace DnD.DnD_5e
{
    public class Spell : Skill_DnD_5e
    {
        bool IsRituel;

        public Spell(ActivContext Context):base(Context)
        {
            this.Context = Context;
        }
    }
}
