using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.WebRTC;

namespace RPG_System.Networking
{
    // Gère une seule connexion WebRTC vers un peer, avec data-channel, ICE buffering,
    // états de connexion et collecte périodique de statistiques (RTT, jitter, pertes…).
    public class PeerConnection : IDisposable
    {
        public string PeerId { get; }
        public RTCPeerConnection Connection { get; }
        public RTCDataChannel DataChannel { get; private set; }

        // True dès que la connexion est établie.
        public bool IsConnected { get; private set; }

        public event Action<string> OnConnectionEstablished;
        public event Action<string> OnConnectionLost;
        public event Action<string, RTCPeerConnectionState> OnConnectionStateChanged;
        public event Action<string, PeerStats> OnStatsUpdated;

        // ICE candidates reçues avant SetRemoteDescription.
        public List<RTCIceCandidateInit> PendingRemoteCandidates { get; } = new();

        private bool _remoteDescSet;
        private readonly List<RTCIceCandidateInit> _localIceCandidates = new();
        private readonly RTCConfiguration _config = new() { iceServers = Array.Empty<RTCIceServer>() };

        private bool _statsLoopRunning;

        public PeerConnection(string peerId, bool isInitiator)
        {
            PeerId = peerId;
            Connection = new RTCPeerConnection(ref _config);

            // Rassembler les ICE locaux
            Connection.OnIceCandidate = c =>
            {
                if (c != null)
                {
                    _localIceCandidates.Add(new RTCIceCandidateInit
                    {
                        candidate     = c.Candidate,
                        sdpMid        = c.SdpMid,
                        sdpMLineIndex = c.SdpMLineIndex
                    });
                }
            };

            // Suivi de l'état de connexion
            Connection.OnConnectionStateChange = state =>
            {
                OnConnectionStateChanged?.Invoke(PeerId, state);

                if (state == RTCPeerConnectionState.Connected)
                {
                    IsConnected = true;
                    OnConnectionEstablished?.Invoke(PeerId);
                }
                else if ((state == RTCPeerConnectionState.Disconnected ||
                          state == RTCPeerConnectionState.Failed ||
                          state == RTCPeerConnectionState.Closed)
                         && IsConnected)
                {
                    IsConnected = false;
                    OnConnectionLost?.Invoke(PeerId);
                }
            };

            // DataChannel
            if (isInitiator)
                InitDataChannel(Connection.CreateDataChannel("default"));
            else
                Connection.OnDataChannel = ch => InitDataChannel(ch);
        }

        private void InitDataChannel(RTCDataChannel channel)
        {
            DataChannel = channel;
            DataChannel.OnMessage = bytes =>
            {
                var raw = Encoding.UTF8.GetString(bytes);
                P2PNetwork.HandleRawMessage(PeerId, raw);
            };
        }

        // --- Signalisation SDP/ICE ---
        public RTCSessionDescriptionAsyncOperation CreateOffer()  => Connection.CreateOffer();
        public RTCSessionDescriptionAsyncOperation CreateAnswer() => Connection.CreateAnswer();

        public RTCSetSessionDescriptionAsyncOperation SetLocalDescription(RTCSessionDescription desc) =>
            Connection.SetLocalDescription(ref desc);

        public RTCSetSessionDescriptionAsyncOperation SetRemoteDescription(RTCSessionDescription desc)
        {
            var op = Connection.SetRemoteDescription(ref desc);
            _remoteDescSet = true;

            foreach (var init in PendingRemoteCandidates)
                Connection.AddIceCandidate(new RTCIceCandidate(init));
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

        public RTCIceCandidateInit[] GetLocalIceCandidates() =>
            _localIceCandidates.ToArray();

        // --- DataChannel send/receive ---
        public void Send(string message)
        {
            if (DataChannel?.ReadyState == RTCDataChannelState.Open)
                DataChannel.Send(Encoding.UTF8.GetBytes(message));
        }

        /// Renégociation ICE (ice-restart). La signalisation du nouvel offer
        /// doit être gérée par P2PNetwork.
        public RTCSessionDescriptionAsyncOperation RestartIce() =>
            Connection.CreateOffer();

        // --- Statistiques WebRTC via GetStats() ---
        public void StartStatsLoop(int intervalMs = 5000)
        {
            if (_statsLoopRunning) return;
            _statsLoopRunning = true;

            _ = Task.Run(async () =>
            {
                while (_statsLoopRunning)
                {
                    try
                    {
                        // Appel GetStats() et attente manuelle
                        var statsOp = Connection.GetStats();
                        while (!statsOp.IsDone)
                            await Task.Yield();

                        var report = statsOp.Value;
                        var stats  = new PeerStats();

                        // Parcours de chaque RTCStats dans le report
                        foreach (var kv in report.Stats)
                        {
                            var r    = kv.Value;
                            var dict = r.Dict; // Unity.WebRTC expose .Dict, pas .Values

                            // RTT (roundTripTime) en ms
                            if (dict.TryGetValue("roundTripTime", out var rtObj) &&
                                double.TryParse(rtObj?.ToString(), out var rtt))
                            {
                                stats.RttMs = (int)Math.Round(rtt * 1000);
                            }
                            // Jitter en ms
                            if (dict.TryGetValue("jitter", out var jtObj) &&
                                double.TryParse(jtObj?.ToString(), out var jitter))
                            {
                                stats.JitterMs = jitter * 1000;
                            }
                            // Packets sent
                            if (dict.TryGetValue("packetsSent", out var psObj) &&
                                ulong.TryParse(psObj?.ToString(), out var sent))
                            {
                                stats.PacketsSent = sent;
                            }
                            // Packets received
                            if (dict.TryGetValue("packetsReceived", out var prObj) &&
                                ulong.TryParse(prObj?.ToString(), out var recv))
                            {
                                stats.PacketsReceived = recv;
                            }
                            // Packets lost
                            if (dict.TryGetValue("packetsLost", out var plObj) &&
                                ulong.TryParse(plObj?.ToString(), out var lost))
                            {
                                stats.PacketsLost = lost;
                            }
                        }

                        // Taux de perte
                        var total = stats.PacketsReceived + stats.PacketsLost;
                        if (total > 0)
                            stats.PacketLossRate = (double)stats.PacketsLost / total;

                        OnStatsUpdated?.Invoke(PeerId, stats);
                    }
                    catch
                    {
                        // Ignorer les erreurs ponctuelles
                    }

                    await Task.Delay(intervalMs);
                }
            });
        }

        public void StopStatsLoop() => _statsLoopRunning = false;

        // --- Cleanup ---
        public void Dispose()
        {
            StopStatsLoop();
            try
            {
                DataChannel?.Close();
                DataChannel?.Dispose();
                Connection?.Close();
                Connection?.Dispose();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[{PeerId}] Dispose error: {ex}");
            }
        }
    }

    /// Contient les métriques extraites via RTCPeerConnection.GetStats().
    public class PeerStats
    {
        public int    RttMs;
        public double JitterMs;
        public double PacketLossRate;
        public ulong  PacketsSent;
        public ulong  PacketsReceived;
        public ulong  PacketsLost;
    }
}
