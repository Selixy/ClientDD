using Selixy_Utils;

namespace RPG_System.API
{
    public static partial class PublicAPI
    {
        [RpcHandler(1)]
        public static byte[] GetConnectionCode(string peerId)
        {
            string code = PrivateAPI.GetConnectionCode(peerId);
            return ByteUtils.ToBytes(code);
        }
    }
}
