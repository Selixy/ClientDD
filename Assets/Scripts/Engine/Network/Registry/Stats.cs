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
    }
}
