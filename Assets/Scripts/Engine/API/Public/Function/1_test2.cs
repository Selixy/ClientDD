using Selixy_Utils;

namespace RPG_System.Networking.Public
{
    public static partial class PublicAPI
    {
        [RpcHandler(1)]
        public static byte[] Test2(float f, int i, string msg)
        {
            return ByteUtils.ToBytes(f * i, msg.Length);
        }
    }
}
