// NetworkRegistry.Stats.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RPG_System.Networking
{
    public static partial class NetworkRegistry
    {
        private static readonly Timer _rankTimer;

        static NetworkRegistry()
        {
            _rankTimer = new Timer(
                _ => RankMachin.RecomputeAll(),
                null,
                dueTime: 0,
                period: 5000
            );
        }

        public static void TrackClient(string id)
        {
            ActiveClients[id] = new RankMachin(id);
        }

        public static void UntrackClient(string id)
        {
            ActiveClients.Remove(id);
        }

        public static float GetAverageNetworkScore()
        {
            var scores = Peers.Values
                .Select(p => p.Stats.NetworkScore);
            return scores.Any() ? (float)scores.Average() : 0f;
        }
    }
}
