using System;
using System.Collections.Generic;
using Unity.WebRTC;

namespace RPG_System.Networking
{
    /// Registre global contenant toutes les informations de réseau :
    /// connexions, rôles, capacités, etc.
    public static class NetworkRegistry
    {
        /// Connexions actives : dictionnaire liant un ID utilisateur à sa PeerConnection.
        public static Dictionary<string, PeerConnection> Peers { get; } = new();

        /// Rôle principal : identifiant du joueur actuellement responsable des calculs principaux (MJ, leader logique…).
        public static string LeaderId { get; set; } = null;

        /// Joueurs jouant un rôle de relai réseau (passage de paquets).
        public static HashSet<string> Relays { get; } = new();

        /// Données sur les performances connues de chaque utilisateur (CPU, RAM libre, ping moyen…)
        /// Ces données peuvent venir d’un benchmark ou d’un ping périodique.
        public static Dictionary<string, NetworkClientStats> ClientStats { get; } = new();

        /// Historique des connexions, utile pour le debug ou les stats.
        public static List<ConnectionLogEntry> ConnectionHistory { get; } = new();

        /// Enregistre une nouvelle connexion au réseau.
        public static void RegisterPeer(string id, PeerConnection connection)
        {
            Peers[id] = connection;
            ConnectionHistory.Add(new ConnectionLogEntry
            {
                PeerId = id,
                Time = DateTime.UtcNow,
                Event = ConnectionEvent.Connected
            });
        }

        /// Déconnecte un utilisateur du réseau.
        public static void UnregisterPeer(string id)
        {
            if (Peers.Remove(id))
            {
                Relays.Remove(id);
                ClientStats.Remove(id);

                ConnectionHistory.Add(new ConnectionLogEntry
                {
                    PeerId = id,
                    Time = DateTime.UtcNow,
                    Event = ConnectionEvent.Disconnected
                });
            }

            if (LeaderId == id)
                LeaderId = null;
        }
    }

    /// Informations sur les capacités du client, à envoyer lors de la négociation.
    public class NetworkClientStats
    {
        public int   PingMs;
        public float CPULoadPercent;
        public float RAMFreeMB;
        public int   LogicalCoreCount;
    }

    /// Événement de connexion à enregistrer pour debug/statistiques.
    public class ConnectionLogEntry
    {
        public string PeerId;
        public DateTime Time;
        public ConnectionEvent Event;
    }

    public enum ConnectionEvent
    {
        Connected,
        Disconnected
    }
}
