using Selixy_Utils;

namespace RPG_System.API
{
    public static partial class PublicAPI
    {
        [RpcHandler(4)]
        public static byte[] SendRawPing(string peerId)
        {
            bool success = PrivateAPI.SendRawPing(peerId);
            return ByteUtils.ToBytes(success);
        }
    }
}
