using System.Collections.Generic;
using System.Linq;

namespace DnD.DnD_5e
{
    public class InitiativeRequest : Request<int>
    {
        public Entity_DnD_5e Target { get; }

        public InitiativeRequest(Entity_DnD_5e target)
        {
            Target = target;
        }

        public override void Execute()
        {
            int Initiative = Target.RequestInitiativeRoll();
            Resolve(Initiative);
        }
    }



    public class MultiInitiativeRequest : Request<Dictionary<Entity_DnD_5e, int>>
    {
        private readonly List<Entity_DnD_5e> _targets;
        private readonly Dictionary<Entity_DnD_5e, int> _results = new();
        private int _remaining;

        public MultiInitiativeRequest(List<Entity_DnD_5e> targets)
        {
            _targets = targets;
            _remaining = targets.Count;
        }

        public override void Execute()
        {
            foreach (var target in _targets)
            {
                var req = new InitiativeRequest(target);
                req.OnResolved += (score) =>
                {
                    _results[target] = score;
                    _remaining--;

                    if (_remaining == 0)
                    {
                        Result = _results;
                        IsFinished = true;
                    }
                };

                RequestSystem.Enqueue(req);
            }
        }
    }
}