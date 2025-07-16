using RPG_System.Networking;

namespace RPG_System.API
{
    public static partial class PrivateAPI
    {
        public static string FinalizeConnection(string encodedAnswer)
        {
            var task = P2PNetwork.FinalizeConnectionAsync(encodedAnswer);
            task.GetAwaiter().GetResult();
            return "ok";
        }
    }
}
