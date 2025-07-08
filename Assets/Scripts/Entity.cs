using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace RPG_System
{
    public class Entity
    {
        public string      Name      { get; private set; }
        public Vector3     Position  { get; private set; }

        public List<Item>  Inventory { get; private set; }
        public List<Item>  Equipped  { get; private set; }

        public List<Class> Classes   { get; private set; }
        public List<State> States    { get; private set; }

        // tmp
        private Dictionary<string, int> StateSummary = new();

        // Constructeur
        public Entity(string       Name      = "Unknown Entity"
                     ,Vector3?     position  = null
                     ,List<Class>  classes   = null
                     ,List<State>  states    = null
                     ,List<Item>   inventory = null
                     ,List<Item>   equipped  = null
                     )
        {
            this.Name      = Name;
            this.Position  = position  ?? Vector3.Zero;
            this.Inventory = inventory ?? new List<Item>();
            this.Equipped  = equipped  ?? new List<Item>();
            this.Classes   = classes   ?? new List<Class>();
            this.States    = states    ?? new List<State>();
            
            UpdateStateSummary();
        }

        // States Logic
        public void AddState(State state)
        {
            state.Owner = this;
            States.Add(state);
            UpdateStateSummary();
        }

        public void RemoveState(State state)
        {
            States.Remove(state);
            UpdateStateSummary();
        }

        public void UpdateStateSummary()
        {
            StateSummary = States
                .GroupBy(s => s.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(s => s.Value)
                );
        }

        public int GetState(string name)
        {
            return StateSummary.TryGetValue(name, out var value) ? value : 0;
        }

        public void StartTurn()
        {
            Classes.ForEach(cls => cls.StartTurn());
            States.ForEach(state => state.StartTurn());
        }

        public void EndTurn()
        {
            Classes.ForEach(cls => cls.EndTurn());
            States.ForEach(state => state.EndTurn());
        }
    }
}