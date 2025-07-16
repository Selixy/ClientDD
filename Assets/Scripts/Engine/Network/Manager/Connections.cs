using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.WebRTC;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        // helpers pour await des opérations WebRTC avec annulation
        private static async Task AwaitDone(RTCSessionDescriptionAsyncOperation op, CancellationToken ct)
        {
            while (!op.IsDone)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }

        private static async Task AwaitDone(RTCSetSessionDescriptionAsyncOperation op, CancellationToken ct)
        {
            while (!op.IsDone)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }

        // MJ : génère et met en cache l’offre SDP+ICE pour peerId
        public static async Task GenerateInvitationAsync(string peerId, CancellationToken ct = default)
        {
            var peer = new PeerConnection(peerId, isInitiator: true);
            NetworkRegistry.RegisterPeer(peerId, peer);
            peer.OnConnectionEstablished += id => OnPeerConnected?.Invoke(id);

            try
            {
                var offerOp = peer.CreateOffer();
                await AwaitDone(offerOp, ct).ConfigureAwait(false);

                var localOp = peer.SetLocalDescription(offerOp.Desc);
                await AwaitDone(localOp, ct).ConfigureAwait(false);

                OfferCache[peerId] = new OfferMessage
                {
                    PeerId        = peerId,
                    Sdp           = offerOp.Desc.sdp,
                    IceCandidates = peer.GetLocalIceCandidates()
                };
            }
            catch (OperationCanceledException)
            {
                peer.Dispose();
                throw;
            }
            catch (Exception ex)
            {
                peer.Dispose();
                throw new InvalidOperationException(
                    $"Échec génération invitation pour '{peerId}'", ex);
            }
        }

        // Joueur : reçoit l’offre encodée, crée la réponse SDP+ICE
        public static async Task ReceiveInvitationAsync(string encodedOffer, CancellationToken ct = default)
        {
            var offer = SignalMessageCodec.Decode<OfferMessage>(encodedOffer);
            var peer  = new PeerConnection(offer.PeerId, isInitiator: false);
            NetworkRegistry.RegisterPeer(offer.PeerId, peer);
            peer.OnConnectionEstablished += id => OnPeerConnected?.Invoke(id);

            try
            {
                var remoteDesc = new RTCSessionDescription
                {
                    type = RTCSdpType.Offer,
                    sdp  = offer.Sdp
                };
                var remoteOp = peer.SetRemoteDescription(remoteDesc);
                await AwaitDone(remoteOp, ct).ConfigureAwait(false);

                foreach (var ice in offer.IceCandidates)
                    peer.AddRemoteIceCandidate(ice);

                var answerOp = peer.CreateAnswer();
                await AwaitDone(answerOp, ct).ConfigureAwait(false);

                var localOp = peer.SetLocalDescription(answerOp.Desc);
                await AwaitDone(localOp, ct).ConfigureAwait(false);

                var answerMsg = new AnswerMessage
                {
                    PeerId        = offer.PeerId,
                    Sdp           = answerOp.Desc.sdp,
                    IceCandidates = peer.GetLocalIceCandidates()
                };
                var code = SignalMessageCodec.Encode(answerMsg);
                // transmettez 'code' au MJ via votre signaling
            }
            catch (OperationCanceledException)
            {
                peer.Dispose();
                throw;
            }
            catch (Exception ex)
            {
                peer.Dispose();
                throw new InvalidOperationException(
                    $"Échec réception invitation pour '{offer.PeerId}'", ex);
            }
        }

        // MJ : finalise la connexion après réception de l’answer encodée
        public static async Task FinalizeConnectionAsync(string encodedAnswer, CancellationToken ct = default)
        {
            var answer = SignalMessageCodec.Decode<AnswerMessage>(encodedAnswer);

            if (!NetworkRegistry.Peers.TryGetValue(answer.PeerId, out var peer))
                throw new KeyNotFoundException(
                    $"Aucune offre en cache pour '{answer.PeerId}'");

            try
            {
                var remoteDesc = new RTCSessionDescription
                {
                    type = RTCSdpType.Answer,
                    sdp  = answer.Sdp
                };
                var remoteOp = peer.SetRemoteDescription(remoteDesc);
                await AwaitDone(remoteOp, ct).ConfigureAwait(false);

                foreach (var ice in answer.IceCandidates)
                    peer.AddRemoteIceCandidate(ice);
            }
            catch (OperationCanceledException)
            {
                peer.Dispose();
                throw;
            }
            catch (Exception ex)
            {
                peer.Dispose();
                throw new InvalidOperationException(
                    $"Échec finalisation pour '{answer.PeerId}'", ex);
            }
        }
    }
}
