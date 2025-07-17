using Selixy_Utils;

namespace RPG_System.API
{
    public static partial class PublicAPI
    {
        [RpcHandler(1)]
        public static byte[] GetConnectionCode()
        {
            string code = PrivateAPI.GetConnectionCode();
            return ByteUtils.ToBytes(code);
        }
    }
}
