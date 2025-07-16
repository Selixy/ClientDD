using Selixy_Utils;

namespace RPG_System.API
{
    public static partial class PublicAPI
    {
        [RpcHandler(3)]
        public static byte[] FinalizeConnection(string encodedAnswer)
        {
            string result = PrivateAPI.FinalizeConnection(encodedAnswer);
            return ByteUtils.ToBytes(result);
        }
    }
}
