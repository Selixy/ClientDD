// Registry.Core.cs
using System;
using System.Collections.Generic;

namespace RPG_System.Networking
{
    public static partial class NetworkRegistry
    {
        // Connexions actives : ID utilisateur → PeerConnection.
        public static Dictionary<string, PeerConnection> Peers         { get; } = new();

        public static Dictionary<string, RankMachin> ActiveClients     { get; } = new();

        // Historique des connexions (Connected / Disconnected).
        public static List<ConnectionLogEntry> ConnectionHistory       { get; } = new();

        public static void RegisterPeer(string id, PeerConnection connection)
        {
            // Stocke la connexion
            Peers[id] = connection;

            // Démarre la collecte de stats dans la PeerConnection
            connection.StartStatsLoop();

            // Log l'événement Connected
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
                // Retire ce peer des relays éventuels
                Relays.Remove(id);

                // Log l'événement Disconnected
                ConnectionHistory.Add(new ConnectionLogEntry
                {
                    PeerId = id,
                    Time   = DateTime.UtcNow,
                    Event  = ConnectionEvent.Disconnected
                });

                // Si c'était le leader, on réinitialise
                if (LeaderId == id)
                    LeaderId = null;
            }
        }
    }
}
