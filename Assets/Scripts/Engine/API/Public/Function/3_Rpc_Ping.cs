using Selixy_Utils;

namespace RPG_System.API
{
    public static partial class PublicAPI
    {
        [RpcHandler(3)]
        public static byte[] Ping(string peerId)
        {
            (bool success, int time) = PrivateAPI.Ping(peerId);
            return ByteUtils.ToBytes(success, time);
        }
    }
}
