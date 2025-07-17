using Selixy_Utils;

namespace RPG_System.API
{
    public static partial class PublicAPI
    {
        [RpcHandler(4)]
        public static byte[] Ping(string peerId)
        {
            (bool success, int time) = PrivateAPI.Ping(peerId);
            return ByteUtils.ToBytes(success, time);
        }
    }
}
