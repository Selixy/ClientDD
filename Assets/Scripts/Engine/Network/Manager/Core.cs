using System;
using System.Collections.Concurrent;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        // déclenché quand un peer est complètement connecté (data-channel ouvert)
        public static event Action<string> OnPeerConnected;

        // cache des offres SDP+ICE générées par le MJ pour la mesh
        internal static readonly ConcurrentDictionary<string, OfferMessage> OfferCache
            = new ConcurrentDictionary<string, OfferMessage>();
    }
}
