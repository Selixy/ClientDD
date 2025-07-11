using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.WebRTC;

namespace RPG_System.Networking
{
    public class PeerConnection : IDisposable
    {
        public string PeerId { get; }
        public RTCPeerConnection Connection { get; }
        public RTCDataChannel DataChannel { get; private set; }
        public bool IsConnected { get; private set; }
        public NetworkClientStats Stats;

        public event Action<string> OnConnectionEstablished;
        public event Action<string> OnConnectionLost;
        public event Action<string, RTCPeerConnectionState> OnConnectionStateChanged;

        private readonly List<RTCIceCandidateInit> _localIceCandidates = new();
        public List<RTCIceCandidateInit> PendingRemoteCandidates { get; } = new();
        private bool _remoteDescSet;
        private readonly RTCConfiguration _config = new() { iceServers = Array.Empty<RTCIceServer>() };
        private bool _statsLoopRunning;

        public PeerConnection(string peerId, bool isInitiator)
        {
            PeerId = peerId;
            Connection = new RTCPeerConnection(ref _config);
            Stats = new NetworkClientStats();

            Connection.OnIceCandidate = c =>
            {
                if (c != null)
                    _localIceCandidates.Add(new RTCIceCandidateInit
                    {
                        candidate     = c.Candidate,
                        sdpMid        = c.SdpMid,
                        sdpMLineIndex = c.SdpMLineIndex
                    });
            };

            Connection.OnConnectionStateChange = state =>
            {
                OnConnectionStateChanged?.Invoke(PeerId, state);
                if (state == RTCPeerConnectionState.Connected)
                {
                    IsConnected = true;
                    OnConnectionEstablished?.Invoke(PeerId);
                    StartStatsLoop();
                }
                else if ((state == RTCPeerConnectionState.Disconnected ||
                          state == RTCPeerConnectionState.Failed ||
                          state == RTCPeerConnectionState.Closed) && IsConnected)
                {
                    IsConnected = false;
                    OnConnectionLost?.Invoke(PeerId);
                    StopStatsLoop();
                }
            };

            if (isInitiator)
                InitDataChannel(Connection.CreateDataChannel("default"));
            else
                Connection.OnDataChannel = ch => InitDataChannel(ch);
        }

        private void InitDataChannel(RTCDataChannel channel)
        {
            DataChannel = channel;
            DataChannel.OnMessage = bytes =>
                P2PNetwork.HandleRawMessage(PeerId, Encoding.UTF8.GetString(bytes));
        }

        public RTCSessionDescriptionAsyncOperation CreateOffer()  => Connection.CreateOffer();
        public RTCSessionDescriptionAsyncOperation CreateAnswer() => Connection.CreateAnswer();
        public RTCSetSessionDescriptionAsyncOperation SetLocalDescription(RTCSessionDescription desc) =>
            Connection.SetLocalDescription(ref desc);

        public RTCSetSessionDescriptionAsyncOperation SetRemoteDescription(RTCSessionDescription desc)
        {
            var op = Connection.SetRemoteDescription(ref desc);
            _remoteDescSet = true;
            foreach (var ice in PendingRemoteCandidates)
                Connection.AddIceCandidate(new RTCIceCandidate(ice));
            PendingRemoteCandidates.Clear();
            return op;
        }

        public void AddRemoteIceCandidate(RTCIceCandidateInit candidate)
        {
            if (_remoteDescSet)
                Connection.AddIceCandidate(new RTCIceCandidate(candidate));
            else
                PendingRemoteCandidates.Add(candidate);
        }

        public RTCIceCandidateInit[] GetLocalIceCandidates() => _localIceCandidates.ToArray();

        public void Send(string message)
        {
            if (DataChannel?.ReadyState == RTCDataChannelState.Open)
                DataChannel.Send(Encoding.UTF8.GetBytes(message));
        }

        public RTCSessionDescriptionAsyncOperation RestartIce() => Connection.CreateOffer();

        public void StartStatsLoop(int intervalMs = 5000)
        {
            if (_statsLoopRunning) return;
            _statsLoopRunning = true;

            _ = Task.Run(async () =>
            {
                while (_statsLoopRunning)
                {
                    var op = Connection.GetStats();
                    while (!op.IsDone) await Task.Yield();
                    var report = op.Value;

                    foreach (var kv in report.Stats)
                    {
                        var dict = kv.Value.Dict;

                        if (dict.TryGetValue("roundTripTime", out var rt) &&
                            double.TryParse(rt.ToString(), out var rtt))
                            Stats.PingMs = (int)Math.Round(rtt * 1000);

                        if (dict.TryGetValue("jitter", out var jt) &&
                            double.TryParse(jt.ToString(), out var jitter))
                            Stats.JitterMs = (float)(jitter * 1000);

                        if (dict.TryGetValue("packetsSent", out var ps) &&
                            ulong.TryParse(ps.ToString(), out var sent))
                            Stats.PacketsSent = sent;

                        if (dict.TryGetValue("packetsReceived", out var pr) &&
                            ulong.TryParse(pr.ToString(), out var recv))
                            Stats.PacketsReceived = recv;

                        if (dict.TryGetValue("packetsLost", out var pl) &&
                            ulong.TryParse(pl.ToString(), out var lost))
                            Stats.PacketsLost = lost;
                    }

                    var total = Stats.PacketsReceived + Stats.PacketsLost;
                    Stats.PacketLossRate = (total > 0)
                        ? (double)Stats.PacketsLost / total
                        : 0;

                    await Task.Delay(intervalMs);
                }
            });
        }

        public void StopStatsLoop() => _statsLoopRunning = false;

        public void Dispose()
        {
            StopStatsLoop();
            DataChannel?.Close();
            DataChannel?.Dispose();
            Connection.Close();
            Connection.Dispose();
        }
    }
}