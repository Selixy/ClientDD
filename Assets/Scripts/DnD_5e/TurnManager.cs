using System.Collections.Generic;
using System.Linq;

namespace DnD.DnD_5e
{
    public static class TurnManager
    {
        private static Queue<Entity_DnD_5e> _order;
        private static Entity_DnD_5e _current;

        public static void BeginCombat(List<Entity_DnD_5e> entities)
        {
            var rollAll = new MultiInitiativeRequest(entities);
            rollAll.OnResolved += (results) =>
            {
                var sorted = results
                    .OrderByDescending(kvp => kvp.Value)
                    .Select(kvp => kvp.Key);

                _order = new Queue<Entity_DnD_5e>(sorted);

                NextTurn();
            };

            RequestSystem.Enqueue(rollAll);
        }

        public static void NextTurn()
        {
            if (_order.Count == 0)
            {
                BeginCombat(EntityRegistry.All.OfType<Entity_DnD_5e>().ToList());
                return;
            }

            _current = _order.Dequeue();
            _current.StartTurn();
        }

        public static void EndTurnIfReady()
        {
            if (RequestSystem.IsIdle)
                NextTurn();
        }
    }
}
