using Selixy_Utils;

namespace RPG_System.API
{
    public static partial class PublicAPI
    {
        [RpcHandler(5)]
        public static byte[] PingLoopback(string peerId)
        {
            object result = PrivateAPI.PingLoopback(peerId);
            return ByteUtils.ToBytes(result);
        }
    }
}
