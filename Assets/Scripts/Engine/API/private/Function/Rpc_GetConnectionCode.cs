using RPG_System.Networking;

namespace RPG_System.API
{
    public static partial class PrivateAPI
    {
        public static string GetConnectionCode(string peerId)
        {
            var task = P2PNetwork.GenerateInvitationAsync(peerId);
            task.GetAwaiter().GetResult();
            var offer = P2PNetwork.OfferCache[peerId];
            return SignalMessageCodec.Encode(offer);
        }
    }
}
