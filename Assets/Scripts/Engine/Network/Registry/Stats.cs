// NetworkRegistry.Stats.cs
namespace RPG_System.Networking
{
    public class NetworkClientStats
    {
        // WebRTC metrics
        public int    PingMs;
        public float  JitterMs;
        public float  PacketLossRate;
        public ulong  PacketsSent;
        public ulong  PacketsReceived;
        public ulong  PacketsLost;

        // CPU load: process and system
        public float CPUUsagePercent;
        public float SystemCPUUsagePercent;
        public float SystemCPUAvailablePercent => 100f - SystemCPUUsagePercent;

        // RAM usage
        public float RAMUsedMB;
        public float RAMTotalMB;
        public float RAMUsagePercent => RAMTotalMB > 0 ? (RAMUsedMB / RAMTotalMB) * 100f : 0f;
        public float RAMAvailablePercent => 100f - RAMUsagePercent;

        // Quality score [0–100]
        public float NetworkScore
        {
            get
            {
                float score = 100f;
                score -= PingMs * 0.1f;
                score -= JitterMs;
                score -= PacketLossRate * 50f;
                return score < 0f ? 0f : (score > 100f ? 100f : score);
            }
        }

        // Computation capacity score [0–100]
        public float ComputationScore
        {
            get
            {
                float loadFactor  = 1f - (CPUUsagePercent  / 100f);
                float availFactor =  SystemCPUAvailablePercent / 100f;
                float score       = (loadFactor + availFactor) * 50f;
                return score < 0f ? 0f : (score > 100f ? 100f : score);
            }
        }
    }
}
