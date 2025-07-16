using RPG_System.Networking;

namespace RPG_System.API
{
    public static partial class PrivateAPI
    {
        public static bool SendRawPing(string peerId)
        {
            return P2PNetwork.SendRawPing(peerId, 1000).GetAwaiter().GetResult();
        }
    }
}
