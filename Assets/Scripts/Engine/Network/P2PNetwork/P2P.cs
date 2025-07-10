using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.WebRTC;

namespace RPG_System.Networking
{
    public static class P2PNetwork
    {
        // Connexions actives vers d'autres joueurs
        private static readonly Dictionary<string, PeerConnection> Peers = new();

        // Cache d'offres envoyées, pour permettre au MJ de les redistribuer à d'autres joueurs
        private static readonly Dictionary<string, OfferMessage> OfferCache = new();

        public static event Action<string, string> OnMessageReceived;
        public static event Action<string> OnPeerConnected;

        /// Appelée par le MJ pour créer une offre de connexion à un joueur.
        /// Cette offre est affichée encodée pour être copiée/transmise.
        public static async Task GenerateInvitationAsync(string peerId)
        {
            var peer = new PeerConnection(peerId, isInitiator: true);
            Peers[peerId] = peer;

            peer.OnMessageReceived += msg => OnMessageReceived?.Invoke(peerId, msg);
            peer.OnConnectionEstablished += id => OnPeerConnected?.Invoke(id);

            var offerOp = peer.CreateOffer();
            while (!offerOp.IsDone) await Task.Yield();

            var desc = offerOp.Desc;
            peer.Connection.SetLocalDescription(ref desc);

            var message = new OfferMessage
            {
                PeerId = peerId,
                Sdp = desc.sdp,
                IceCandidates = peer.GetLocalIceCandidates()
            };

            OfferCache[peerId] = message;

            string encoded = SignalMessageCodec.Encode(message);
            UnityEngine.Debug.Log($"[MJ] Send this Offer to target {peerId}: {encoded}");
        }

        /// Appelée par un joueur recevant l'invitation du MJ. Crée une réponse à renvoyer.
        public static async Task ReceiveInvitationAsync(string encoded)
        {
            var offer = SignalMessageCodec.Decode<OfferMessage>(encoded);
            var peer = new PeerConnection(offer.PeerId, isInitiator: false);
            Peers[offer.PeerId] = peer;

            peer.OnMessageReceived += msg => OnMessageReceived?.Invoke(offer.PeerId, msg);
            peer.OnConnectionEstablished += id => OnPeerConnected?.Invoke(id);

            var remoteDesc = new RTCSessionDescription
            {
                type = RTCSdpType.Offer,
                sdp = offer.Sdp
            };

            var setRemoteOp = peer.Connection.SetRemoteDescription(ref remoteDesc);
            while (!setRemoteOp.IsDone) await Task.Yield();

            foreach (var candidate in offer.IceCandidates)
                peer.AddRemoteIceCandidate(candidate);

            var answerOp = peer.CreateAnswer();
            while (!answerOp.IsDone) await Task.Yield();

            var answerDesc = answerOp.Desc;
            peer.Connection.SetLocalDescription(ref answerDesc);

            var answerMsg = new AnswerMessage
            {
                PeerId = offer.PeerId,
                Sdp = answerDesc.sdp,
                IceCandidates = peer.GetLocalIceCandidates()
            };

            string responseCode = SignalMessageCodec.Encode(answerMsg);
            UnityEngine.Debug.Log($"[Joueur] Send this Answer back to MJ: {responseCode}");
        }

        /// Appelée par le MJ après avoir reçu la réponse du joueur.
        /// Finalise la connexion WebRTC.
        public static async Task FinalizeConnectionAsync(string encoded)
        {
            var answer = SignalMessageCodec.Decode<AnswerMessage>(encoded);
            if (!Peers.TryGetValue(answer.PeerId, out var peer))
                throw new Exception($"[MJ] No offer found for peer {answer.PeerId}");

            var remoteDesc = new RTCSessionDescription
            {
                type = RTCSdpType.Answer,
                sdp = answer.Sdp
            };

            var setRemoteOp = peer.Connection.SetRemoteDescription(ref remoteDesc);
            while (!setRemoteOp.IsDone) await Task.Yield();

            foreach (var candidate in answer.IceCandidates)
                peer.AddRemoteIceCandidate(candidate);

            UnityEngine.Debug.Log($"[MJ] Connection with {answer.PeerId} finalized.");
        }

        /// Envoie un message texte à un joueur précis.
        public static void SendTo(string peerId, string message)
        {
            if (Peers.TryGetValue(peerId, out var peer))
                peer.Send(message);
        }

        /// Liste tous les PeerId connectés.
        public static IEnumerable<string> GetConnectedPeers()
        {
            return Peers.Keys;
        }

        /// Le MJ peut obtenir toutes les offres sauf celle d'un joueur spécifique.
        /// Cela permet d'envoyer les connexions déjà établies à un nouveau venu.
        public static string[] GetAllOffersExcept(string exceptPeerId)
        {
            List<string> offers = new();
            foreach (var kvp in OfferCache)
            {
                if (kvp.Key == exceptPeerId) continue;
                offers.Add(SignalMessageCodec.Encode(kvp.Value));
            }
            return offers.ToArray();
        }

        /// Le MJ peut appeler cette méthode pour envoyer l'offre d'un nouveau joueur
        /// à tous les autres déjà connectés (construction du graphe P2P full-mesh).
        public static void DistributeNewPeer(string newPeerId)
        {
            if (!OfferCache.TryGetValue(newPeerId, out var offer))
            {
                UnityEngine.Debug.LogWarning($"[MJ] No cached offer for {newPeerId}");
                return;
            }

            string encoded = SignalMessageCodec.Encode(offer);

            foreach (var otherId in Peers.Keys)
            {
                if (otherId == newPeerId) continue;

                UnityEngine.Debug.Log($"[MJ] Send offer of {newPeerId} to {otherId}: {encoded}");
            }
        }
    }
}