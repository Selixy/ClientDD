using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public struct State
    {
        public int Str { get; set; }
        public int Dex { get; set; }
        public int Con { get; set; }
        public int Int { get; set; }
        public int Wis { get; set; }
        public int Cha { get; set; }

        public State(int str, int dex, int con, int intel, int wis, int cha)
        {
            Str = str;
            Dex = dex;
            Con = con;
            Int = intel;
            Wis = wis;
            Cha = cha;
        }
        
        public State mod
        {
            get
            {            
                int mStr  = (int)System.Math.Floor((Str - 10) / 2.0);
                int mDex  = (int)System.Math.Floor((Dex - 10) / 2.0);
                int mCon  = (int)System.Math.Floor((Con - 10) / 2.0);
                int mInt  = (int)System.Math.Floor((Int - 10) / 2.0);
                int mWis  = (int)System.Math.Floor((Wis - 10) / 2.0);
                int mCha  = (int)System.Math.Floor((Cha - 10) / 2.0);

                return new State(mStr, mDex, mCon, mInt, mWis, mCha);
            }
        }
    }

    public class Entity_DnD_5e : Entity<Item_DnD_5e, Etat_DnD_5e>
    {
        public string            Race           { get; protected set; }
        public string            Class          { get; protected set; }
        public State             Stats          { get; protected set; }
        public State             Modifiers      { get; protected set; }

        public ActivContext      fightContext   { get; set; }
        public List<State>           Debuffs    { get; protected set; } = new List<State>();


        public Entity_DnD_5e(string name       = "[Unknown Entity]"
                            ,string race       = null
                            ,string className  = null
                            ,int    lvl        = 1
                            ,int    exp        = 0
                            ,int    hpMax      = 1
                            ,State? stats      = null
                            ,State? modifiers  = null
                            ,Inventaire_DnD_5e Inventaire = null
                            )
                            : base(name
                                  ,lvl
                                  ,exp
                                  ,hpMax
                                  ,Inventaire ?? new Inventaire_DnD_5e()
                                  )
        {
            this.Race       = race;
            this.Class      = className;
            this.Stats      = stats      ?? new State(10, 10, 10, 10, 10, 10);
            this.Modifiers  = modifiers  ?? Stats.mod;

        }

    }
}
