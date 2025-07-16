using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        // entrée ACK en cours d’attente
        class AckEntry
        {
            public Action Callback;
            public int Attempts;
            public byte[] Envelope;
            public string PeerId;
            public DateTime TimestampUtc = DateTime.UtcNow;
        }

        static readonly ConcurrentDictionary<Guid, AckEntry> _pendingAcks =
            new ConcurrentDictionary<Guid, AckEntry>();

        static readonly ConcurrentDictionary<Guid, (string peerId, byte[] envelope)> _failedMessages =
            new ConcurrentDictionary<Guid, (string, byte[])>();

        // pour déduplication des GUID reçus
        static readonly HashSet<Guid> _receivedIds = new HashSet<Guid>();

        // purge périodique des ACK et des GUID
        static readonly TimeSpan AckTtl        = TimeSpan.FromMinutes(5);
        static readonly TimeSpan PurgeInterval = TimeSpan.FromMinutes(1);
        static readonly Timer _purgeTimer;

        static P2PNetwork()
        {
            _purgeTimer = new Timer(_ => PurgeExpired(), null,
                PurgeInterval, PurgeInterval);
        }

        static void PurgeExpired()
        {
            var now = DateTime.UtcNow;

            // supprime ACK trop vieux
            foreach (var kv in _pendingAcks)
            {
                if (now - kv.Value.TimestampUtc > AckTtl)
                    _pendingAcks.TryRemove(kv.Key, out _);
            }

            // réinitialise la liste des GUID reçus
            lock (_receivedIds)
            {
                _receivedIds.Clear();
            }
        }

        public static event Action<string, MessageType, byte[]> OnMessageReceived;
        public static event Action<string, Guid>          OnMessageAcknowledged;
        public static event Action<string, Guid>          OnMessageFailed;

        const int MAX_ATTEMPTS   = 3;
        const int RETRY_DELAY_MS = 300;

        public enum MessageType : byte
        {
            ApiCall = 1,
            Audio   = 2
        }

        // envoie un envelope (type+GUID+len+payload) avec retry/ack
        public static void SendMessage(
            string peerId,
            MessageType type,
            byte[] payload,
            Action onAck = null)
        {
            var messageId = Guid.NewGuid();
            var envelope  = BuildEnvelope(type, messageId, payload);

            _pendingAcks[messageId] = new AckEntry {
                Callback = onAck,
                Attempts = 0,
                Envelope = envelope,
                PeerId   = peerId
            };

            _ = RetrySendAsync(messageId);
        }

        static byte[] BuildEnvelope(MessageType type, Guid messageId, byte[] payload)
        {
            int total = 1 + 16 + 2 + 1 + payload.Length;
            var buf   = new byte[total];
            int o = 0;

            buf[o++] = (byte)type;
            Buffer.BlockCopy(messageId.ToByteArray(), 0, buf, o, 16); o += 16;

            ushort len = (ushort)payload.Length;
            buf[o++] = (byte)(len >> 8);
            buf[o++] = (byte)(len & 0xFF);
            buf[o++] = 0; // reserved

            Buffer.BlockCopy(payload, 0, buf, o, payload.Length);
            return buf;
        }

        static async Task RetrySendAsync(Guid messageId)
        {
            if (!_pendingAcks.TryGetValue(messageId, out var entry))
                return;

            while (entry.Attempts < MAX_ATTEMPTS)
            {
                entry.Attempts++;
                if (NetworkRegistry.Peers.TryGetValue(entry.PeerId, out var peer))
                    peer.Send(entry.Envelope);

                await Task.Delay(RETRY_DELAY_MS).ConfigureAwait(false);

                if (!_pendingAcks.ContainsKey(messageId))
                    return;
            }

            if (_pendingAcks.TryRemove(messageId, out entry))
            {
                _failedMessages[messageId] = (entry.PeerId, entry.Envelope);
                OnMessageFailed?.Invoke(entry.PeerId, messageId);
            }
        }

        // traite le raw reçu : ping/pong ou enveloppe+ack
        public static void HandleRawMessageBytes(string peerId, byte[] raw)
        {
            if (raw.Length == 1 && raw[0] == 0x01) // ping
            {
                if (NetworkRegistry.Peers.TryGetValue(peerId, out var p))
                    p.Send(new byte[] { 0x02 });
                return;
            }

            if (raw.Length == 1 && raw[0] == 0x02) // pong
            {
                OnPongReceived?.Invoke(peerId);
                return;
            }

            if (raw.Length < 20) return;

            int o = 0;
            var type = (MessageType)raw[o++];
            var guidBytes = new byte[16];
            Buffer.BlockCopy(raw, o, guidBytes, 0, 16); o += 16;
            var messageId = new Guid(guidBytes);

            ushort length = (ushort)((raw[o++] << 8) | raw[o++]);
            o++; // reserved

            if (o + length > raw.Length) return;

            lock (_receivedIds)
                if (!_receivedIds.Add(messageId))
                    return;

            SendAck(peerId, messageId);

            var payload = new byte[length];
            Buffer.BlockCopy(raw, o, payload, 0, length);
            OnMessageReceived?.Invoke(peerId, type, payload);
        }

        static void SendAck(string peerId, Guid messageId)
        {
            var ack = new byte[1 + 16];
            ack[0] = 255;
            Buffer.BlockCopy(messageId.ToByteArray(), 0, ack, 1, 16);
            if (NetworkRegistry.Peers.TryGetValue(peerId, out var p))
                p.Send(ack);
        }

        // supprime l’entrée pending et invoque le callback
        public static void HandleAckBytes(string peerId, byte[] raw)
        {
            if (raw.Length != 17 || raw[0] != 255) return;

            var guid = new byte[16];
            Buffer.BlockCopy(raw, 1, guid, 0, 16);
            var messageId = new Guid(guid);

            if (_pendingAcks.TryRemove(messageId, out var entry))
            {
                entry.Callback?.Invoke();
                OnMessageAcknowledged?.Invoke(entry.PeerId, messageId);
            }
        }

        // lecture seule des messages échoués
        public static IReadOnlyDictionary<Guid,(string peerId, byte[] envelope)> GetFailedMessages()
            => _failedMessages;
    }
}
