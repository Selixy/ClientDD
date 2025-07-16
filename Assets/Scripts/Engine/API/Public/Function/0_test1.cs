using Selixy_Utils;

namespace RPG_System.Networking.Public
{
    public static partial class PublicAPI
    {
        [RpcHandler(0)]
        public static byte[] Test1(bool flag)
        {
            return flag
                ? ByteUtils.ToBytes(2.0f, 6, "Yolo")
                : ByteUtils.ToBytes("nope");
        }
    }
}
