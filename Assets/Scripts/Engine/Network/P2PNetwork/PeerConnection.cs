using System;
using System.Collections.Generic;
using Unity.WebRTC;

namespace RPG_System.Networking
{
    public class PeerConnection
    {
        public string PeerId { get; private set; }
        public RTCPeerConnection Connection { get; private set; }
        public RTCDataChannel DataChannel { get; private set; }
        public List<RTCIceCandidateInit> PendingRemoteCandidates { get; } = new();

        public event Action<string> OnMessageReceived;
        public event Action<string> OnConnectionEstablished;

        private readonly RTCConfiguration _config = new()
        {
            iceServers = Array.Empty<RTCIceServer>() // pas de relais, full local
        };

        private readonly List<RTCIceCandidateInit> _localIceCandidates = new();

        public PeerConnection(string peerId, bool isInitiator)
        {
            PeerId = peerId;
            Connection = new RTCPeerConnection(ref _config);

            Connection.OnIceCandidate = candidate =>
            {
                if (candidate != null)
                {
                    var candidateInit = new RTCIceCandidateInit
                    {
                        candidate = candidate.Candidate,
                        sdpMid = candidate.SdpMid,
                        sdpMLineIndex = candidate.SdpMLineIndex
                    };
                    _localIceCandidates.Add(candidateInit);
                }
            };

            Connection.OnConnectionStateChange = state =>
            {
                if (state == RTCPeerConnectionState.Connected)
                    OnConnectionEstablished?.Invoke(PeerId);
            };

            if (isInitiator)
            {
                DataChannel = Connection.CreateDataChannel("default");
                SetupDataChannelHandlers(DataChannel);
            }
            else
            {
                Connection.OnDataChannel = channel =>
                {
                    DataChannel = channel;
                    SetupDataChannelHandlers(channel);
                };
            }
        }

        private void SetupDataChannelHandlers(RTCDataChannel channel)
        {
            channel.OnMessage = bytes =>
            {
                string message = System.Text.Encoding.UTF8.GetString(bytes);
                OnMessageReceived?.Invoke(message);
            };
        }

        public RTCSessionDescriptionAsyncOperation CreateOffer()
        {
            return Connection.CreateOffer();
        }

        public RTCSessionDescriptionAsyncOperation CreateAnswer()
        {
            return Connection.CreateAnswer();
        }

        public void SetLocalDescription(RTCSessionDescription desc)
        {
            Connection.SetLocalDescription(ref desc);
        }

        public void SetRemoteDescription(RTCSessionDescription desc)
        {
            Connection.SetRemoteDescription(ref desc);
        }

        public void AddRemoteIceCandidate(RTCIceCandidateInit candidate)
        {
            Connection.AddIceCandidate(new RTCIceCandidate(candidate));
        }

        public RTCIceCandidateInit[] GetLocalIceCandidates()
        {
            return _localIceCandidates.ToArray();
        }

        public void Send(string message)
        {
            if (DataChannel != null && DataChannel.ReadyState == RTCDataChannelState.Open)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(message);
                DataChannel.Send(bytes);
            }
        }
    }
}
