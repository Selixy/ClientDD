namespace RPG_System
{
    public class State
    {
        private Entity _owner;
        public  Entity Owner
        {
            get => _owner;
            set
            {
                if (_owner != null)
                {
                    _owner.RemoveState(this);
                }

                _owner = value;
            }
        }

        public string  Name        { get; private set; }
        public int     Value       { get; private set; }
        public int     MaxValue    { get; private set; }
        public int     Duration    { get; private set; }
        public int     MaxDuration { get; private set; }

        public State(string Name
                    ,Entity Owner       = null
                    ,int    Value       = 0
                    ,int    MaxValue    = 1
                    ,int    Duration    = 0
                    ,int    MaxDuration = 0
                    )
        {
            this.Name        = Name;
            this._owner      = Owner;
            this.Value       = Value;
            this.MaxValue    = MaxValue;
            this.Duration    = Duration;
            this.MaxDuration = MaxDuration;
        }
        public void StartTurn()
        {
            if (this.MaxDuration > 0)
            {
                this.Duration ++;
                if (this.Duration >= this.MaxDuration)
                    this._owner?.RemoveState(this);
            }
        }

        public void EndTurn()
        {
            if (this.MaxDuration < 0)
            {
                this.Duration --;
                if (this.Duration <= this.MaxDuration)
                    this._owner?.RemoveState(this);
            }
        }
    }
}