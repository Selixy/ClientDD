using System;

namespace RPG_System
{
    public class Dice
    {
        public int Value  {get; private set;}
        public string Tag {get; private set;}

        private static readonly Random _rng = new Random();

        public Dice(int v, string t = null)
        {
            this.Value  = v;
            this.Tag    = t;
        }

        public (int Result, string Tag) Roll
        {
            get
            {
                int result = _rng.Next(1, Value + 1);
                return (result, this.Tag);
            }
        }
    }
}