using RPG_System.Networking;

namespace RPG_System.API
{
    public static partial class PrivateAPI
    {
        public static string SubmitConnectionCode(string encoded)
        {
            var t = P2PNetwork.ReceiveInvitationAsync(encoded);
            t.GetAwaiter().GetResult();
            return "ok";
        }
    }
}
