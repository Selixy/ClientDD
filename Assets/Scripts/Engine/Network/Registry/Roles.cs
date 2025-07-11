using System.Collections.Generic;

namespace RPG_System.Networking
{
    public static partial class NetworkRegistry
    {
        /// ID du peer qui tient actuellement le rôle de leader (MJ, calcul…).
        public static string LeaderId { get; set; }

        /// Ensemble des peers désignés comme relais (simple forward de paquets).
        public static HashSet<string> Relays { get; } = new();

        /// Attribue un relay à la liste.
        public static void AddRelay(string peerId) => Relays.Add(peerId);

        /// Retire un relay.
        public static void RemoveRelay(string peerId) => Relays.Remove(peerId);
    }
}
