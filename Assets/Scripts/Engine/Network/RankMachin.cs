using System;
using System.Collections.Generic;
using System.Linq;

namespace RPG_System.Networking
{
    public class RankMachin
    {
        public string ID;
        public float  Score_Network;
        public float  Score_Computation;
        public int    Rank_Network;
        public int    Rank_Computation;

        public RankMachin(string id)
        {
            ID = id;
            RankMachin.RecomputeAll();
        }

        /// Recalcule les rangs Network et Computation pour tous les RankMachin trackés.
        public static void RecomputeAll()
        {
            //RefreshScore();

            var all = NetworkRegistry.ActiveClients.Values.ToList();

            var byNet = all.OrderByDescending(r => r.Score_Network).ToList();
            for (int i = 0; i < byNet.Count; i++)
                byNet[i].Rank_Network = i + 1;

            var byComp = all.OrderByDescending(r => r.Score_Computation).ToList();
            for (int i = 0; i < byComp.Count; i++)
                byComp[i].Rank_Computation = i + 1;
        }

        public void RefreshScore()
        {
            // Computation : localUser si ID identique, sinon on demande par message a l'api
            if (this.ID == User_Info.ID)
            {
                this.Score_Computation = User_Info.ComputationScore;

                // Network : moyenne de tous les NetworkScore des peers connus
                NetworkRegistry.GetAverageNetworkScore();
            }
            else
            {
                this.Score_Computation = 0f;
                this.Score_Network = 0f;
            }
        }
    }
}