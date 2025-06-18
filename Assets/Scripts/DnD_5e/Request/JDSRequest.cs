using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public class JDSRequest : Request<int>
    {
        public Entity_DnD_5e Target { get; }
        public DnDRollType   StatType { get; }
        public ContextRoll   Context { get; }

        public JDSRequest(Entity_DnD_5e target, DnDRollType statType, ContextRoll context)
        {
            Target = target;
            StatType = statType;
            Context = context;
        }

        public override void Execute()
        {
            var (total, _) = Target.RequestJDS(StatType, Context);
            Resolve(total);
        }
    }



    public class MultiJDSRequest : Request<Dictionary<Entity_DnD_5e, int>>
    {
        private readonly List<Entity_DnD_5e> _targets;
        private readonly DnDRollType _stat;
        private readonly ContextRoll _context;

        private readonly Dictionary<Entity_DnD_5e, int> _results = new();
        private int _remaining;

        public MultiJDSRequest(List<Entity_DnD_5e> targets, DnDRollType stat, ContextRoll context)
        {
            _targets  = targets;
            _stat     = stat;
            _context  = context;
            _remaining = targets.Count;
        }

        public override void Execute()
        {
            foreach (var target in _targets)
            {
                var req = new JDSRequest(target, _stat, _context);
                req.OnResolved += (result) =>
                {
                    _results[target] = result;
                    _remaining--;

                    if (_remaining == 0)
                    {
                        this.Result = _results;
                        this.IsFinished = true;
                    }
                };

                RequestSystem.Enqueue(req);
            }
        }
    }

}
