using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RPG_System.Networking
{
    public static partial class NetworkRegistry
    {
        // ID du peer qui tient actuellement le rôle de leader
        public static string LeaderId   { get; set; }

        // ID du peer qui tient actuellement le rôle de calculateur
        public static string computerID { get; set; }

        // Ensemble des peers désignés comme relais (simple forward de paquets).
        public static HashSet<string> Relays { get; } = new();

        // Attribue un relay à la liste.
        public static void AddRelay(string peerId) => Relays.Add(peerId);

        // Retire un relay.
        public static void RemoveRelay(string peerId) => Relays.Remove(peerId);

        // Change le computerID
        public static void DefineComputerID()
        {
            if (computerID != User_Info.ID)
            {
                if (P2PNetwork.SendRawPing(computerID).GetAwaiter().GetResult())
                    return;
            }

            int[] scoreThresholds = { 50, 40, 30, 20, 10, 0 };
            RankMachin best = null;

            foreach (int minScore in scoreThresholds)
            {
                best = ActiveClients.Values
                    .Where(r => r.Score_Network >= minScore && r.ID != computerID)
                    .OrderByDescending(r => r.Rank_Computation)
                    .FirstOrDefault();

                if (best != null)
                    break;
            }

            if (best != null)
            {
                computerID = best.ID;
            }
        }
    }
}
