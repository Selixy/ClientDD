using Selixy_Utils;

namespace RPG_System.API
{
    public static partial class PublicAPI
    {
        [RpcHandler(4)]
        public static byte[] SceneLoader(string sceneName)
        {
            var r = PrivateAPI.SceneLoader(sceneName);
            return ByteUtils.ToBytes(r);
        }
    }
}
