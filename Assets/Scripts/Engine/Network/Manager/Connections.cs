// Collections.cs
using System;
using System.Threading.Tasks;
using Unity.WebRTC;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        // Auxiliaires pour await des opérations WebRTC
        private static async Task AwaitDone(RTCSessionDescriptionAsyncOperation op)
        {
            while (!op.IsDone) await Task.Yield();
        }
        private static async Task AwaitDone(RTCSetSessionDescriptionAsyncOperation op)
        {
            while (!op.IsDone) await Task.Yield();
        }

        /// MJ : Génère et met en cache l’offre SDP+ICE pour peerId.
        public static async Task GenerateInvitationAsync(string peerId)
        {
            var peer = new PeerConnection(peerId, isInitiator: true);
            NetworkRegistry.RegisterPeer(peerId, peer);

            // branchement événements
            peer.OnConnectionEstablished += id  => OnPeerConnected?.Invoke(id);

            // création de l'offre
            var offerOp = peer.CreateOffer();
            await AwaitDone(offerOp);

            // application locale
            var localOp = peer.SetLocalDescription(offerOp.Desc);
            await AwaitDone(localOp);

            // mise en cache dans le Core
            var message = new OfferMessage {
                PeerId        = peerId,
                Sdp           = offerOp.Desc.sdp,
                IceCandidates = peer.GetLocalIceCandidates()
            };
            OfferCache[peerId] = message;

            var code = SignalMessageCodec.Encode(message);
            UnityEngine.Debug.Log($"[MJ] Offer pour {peerId}: {code}");
        }

        /// Joueur : traite l’offre reçue, crée et log la réponse SDP+ICE.
        public static async Task ReceiveInvitationAsync(string encoded)
        {
            var offer = SignalMessageCodec.Decode<OfferMessage>(encoded);
            var peer  = new PeerConnection(offer.PeerId, isInitiator: false);
            NetworkRegistry.RegisterPeer(offer.PeerId, peer);

            peer.OnConnectionEstablished += id  => OnPeerConnected?.Invoke(id);

            // application de l'offre distante
            var remoteDesc = new RTCSessionDescription {
                type = RTCSdpType.Offer,
                sdp  = offer.Sdp
            };
            var remoteOp = peer.SetRemoteDescription(remoteDesc);
            await AwaitDone(remoteOp);

            foreach (var c in offer.IceCandidates)
                peer.AddRemoteIceCandidate(c);

            // création de la réponse
            var answerOp = peer.CreateAnswer();
            await AwaitDone(answerOp);

            var localOp = peer.SetLocalDescription(answerOp.Desc);
            await AwaitDone(localOp);

            var answerMsg = new AnswerMessage {
                PeerId        = offer.PeerId,
                Sdp           = answerOp.Desc.sdp,
                IceCandidates = peer.GetLocalIceCandidates()
            };
            var code = SignalMessageCodec.Encode(answerMsg);
            UnityEngine.Debug.Log($"[Joueur] Answer pour {offer.PeerId}: {code}");
        }

        /// MJ : finalise la connexion après réception de l’Answer.
        public static async Task FinalizeConnectionAsync(string encoded)
        {
            var answer = SignalMessageCodec.Decode<AnswerMessage>(encoded);
            if (!NetworkRegistry.Peers.TryGetValue(answer.PeerId, out var peer))
                throw new Exception($"[MJ] Pas d'offre pour {answer.PeerId}");

            var remoteDesc = new RTCSessionDescription {
                type = RTCSdpType.Answer,
                sdp  = answer.Sdp
            };
            var remoteOp = peer.SetRemoteDescription(remoteDesc);
            await AwaitDone(remoteOp);

            foreach (var c in answer.IceCandidates)
                peer.AddRemoteIceCandidate(c);

            UnityEngine.Debug.Log($"[MJ] Connexion finalisée avec {answer.PeerId}");
        }
    }
}
