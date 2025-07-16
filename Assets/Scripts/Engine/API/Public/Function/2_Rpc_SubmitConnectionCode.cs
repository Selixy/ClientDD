using Selixy_Utils;

namespace RPG_System.API
{
    public static partial class PublicAPI
    {
        [RpcHandler(2)]
        public static byte[] SubmitConnectionCode(string encoded)
        {
            string result = PrivateAPI.SubmitConnectionCode(encoded);
            return ByteUtils.ToBytes(result);
        }
    }
}
