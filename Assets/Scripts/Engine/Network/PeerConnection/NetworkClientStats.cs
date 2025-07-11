namespace RPG_System.Networking
{
    public struct NetworkClientStats
    {
        public int    PingMs;
        public float  JitterMs;
        public double PacketLossRate;
        public ulong  PacketsSent;
        public ulong  PacketsReceived;
        public ulong  PacketsLost;

        public float NetworkScore
        {
            get
            {
                float score = 100f;
                score -= PingMs * 0.1f;
                score -= JitterMs;
                score -= (float)(PacketLossRate * 50f);
                return score < 0f ? 0f : (score > 100f ? 100f : score);
            }
        }
    }
}