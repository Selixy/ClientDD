using System;
using System.Collections.Generic;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        // Événement déclenché quand un peer se connecte (SDP + ICE complet + DataChannel ouvert)
        public static event Action<string> OnPeerConnected;

        // Événement déclenché quand on reçoit un message d'un peer
        public static event Action<string, string> OnMessageReceived;

        // Cache des offres SDP+ICE générées par le MJ, pour la distribution full-mesh
        internal static readonly Dictionary<string, OfferMessage> OfferCache = new();
    }
}
