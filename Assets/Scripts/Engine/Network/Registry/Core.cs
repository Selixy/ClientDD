// Registry.Core.cs
using System;
using System.Collections.Generic;

namespace RPG_System.Networking
{
    public static partial class NetworkRegistry
    {
        /// Connexions actives : ID utilisateur → PeerConnection.
        public static Dictionary<string, PeerConnection> Peers { get; } = new();

        /// Statistiques réseau / machine par peer.
        public static Dictionary<string, NetworkClientStats> ClientStats { get; } = new();

        /// Historique des connexions (Connected / Disconnected).
        public static List<ConnectionLogEntry> ConnectionHistory { get; } = new();

        public static void RegisterPeer(string id, PeerConnection connection)
        {
            // 1) Stocke la connexion
            Peers[id] = connection;

            // 2) Initialise et stocke l'objet stats associé
            var stats = new NetworkClientStats();
            ClientStats[id] = stats;

            // 3) Lance la boucle de collecte de stats
            connection.StartStatsLoop();

            // 4) Abonne-toi à l'événement de mise à jour des stats
            connection.OnStatsUpdated += (peerId, s) =>
            {
                // Remplis les champs de NetworkClientStats
                stats.PingMs         = s.RttMs;
                stats.JitterMs       = (float)s.JitterMs;
                stats.PacketLossRate = (float)s.PacketLossRate;
                stats.PacketsSent    = s.PacketsSent;
                stats.PacketsReceived= s.PacketsReceived;
                stats.PacketsLost    = s.PacketsLost;
            };

            // 5) Log dans l'historique
            ConnectionHistory.Add(new ConnectionLogEntry
            {
                PeerId = id,
                Time   = DateTime.UtcNow,
                Event  = ConnectionEvent.Connected
            });
        }

        public static void UnregisterPeer(string id)
        {
            if (Peers.Remove(id))
            {
                // Arrête la collecte de stats
                ClientStats.Remove(id);
                Relays.Remove(id);

                ConnectionHistory.Add(new ConnectionLogEntry
                {
                    PeerId = id,
                    Time   = DateTime.UtcNow,
                    Event  = ConnectionEvent.Disconnected
                });

                if (LeaderId == id) LeaderId = null;
            }
        }
    }
}
