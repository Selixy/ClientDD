using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        private class AckEntry
        {
            public Action Callback;
            public int Attempts;
            public byte[] Envelope;
            public string PeerId;
        }

        private static readonly ConcurrentDictionary<Guid, AckEntry> _pendingAcks = new();
        private static readonly ConcurrentDictionary<Guid, (string peerId, byte[] envelope)> _failedMessages = new();
        private static readonly HashSet<Guid> _receivedIds = new();

        public static event Action<string, MessageType, byte[]> OnMessageReceived;
        public static event Action<string, Guid> OnMessageAcknowledged;
        public static event Action<string, Guid> OnMessageFailed;

        private const int MAX_ATTEMPTS = 3;
        private const int RETRY_DELAY_MS = 300;

        public enum MessageType : byte
        {
            ApiCall = 1,
            Audio = 2
        }

        /// Envoie un message binaire avec retry et ACK
        public static void SendMessage(string peerId, MessageType type, byte[] payload, Action onAck = null)
        {
            Guid messageId = Guid.NewGuid();
            byte[] envelope = BuildEnvelope(type, messageId, payload);

            var entry = new AckEntry
            {
                Callback = onAck,
                Attempts = 0,
                Envelope = envelope,
                PeerId = peerId
            };
            _pendingAcks[messageId] = entry;

            _ = RetrySendAsync(messageId);
        }

        private static byte[] BuildEnvelope(MessageType type, Guid messageId, byte[] payload)
        {
            byte[] guidBytes = messageId.ToByteArray();
            ushort payloadLength = (ushort)payload.Length;

            byte[] buffer = new byte[1 + 16 + 2 + 1 + payload.Length];
            int offset = 0;

            buffer[offset++] = (byte)type;
            Buffer.BlockCopy(guidBytes, 0, buffer, offset, 16);
            offset += 16;
            buffer[offset++] = (byte)(payloadLength >> 8);
            buffer[offset++] = (byte)(payloadLength & 0xFF);
            buffer[offset++] = 0;
            Buffer.BlockCopy(payload, 0, buffer, offset, payload.Length);

            return buffer;
        }

        private static async Task RetrySendAsync(Guid messageId)
        {
            if (!_pendingAcks.TryGetValue(messageId, out var entry))
                return;

            while (entry.Attempts < MAX_ATTEMPTS)
            {
                entry.Attempts++;
                if (NetworkRegistry.Peers.TryGetValue(entry.PeerId, out var peer))
                    peer.Send(entry.Envelope);

                await Task.Delay(RETRY_DELAY_MS);
                if (!_pendingAcks.ContainsKey(messageId))
                    return;
            }

            if (_pendingAcks.TryRemove(messageId, out entry))
            {
                _failedMessages[messageId] = (entry.PeerId, entry.Envelope);
                OnMessageFailed?.Invoke(entry.PeerId, messageId);
            }
        }

        /// Gère la réception d’un message binaire brut
        public static void HandleRawMessageBytes(string peerId, byte[] raw)
        {
            // Ping brut (0x01) → on répond avec Pong
            if (raw.Length == 1 && raw[0] == 0x01)
            {
                if (NetworkRegistry.Peers.TryGetValue(peerId, out var peer))
                    peer.Send(new byte[] { 0x02 });
                return;
            }
            // Pong brut (0x02) → Reponse
            if (raw.Length == 1 && raw[0] == 0x02)
            {
                OnPongReceived?.Invoke(peerId);
                return;
            }

            if (raw.Length < 20) return;

            int offset = 0;
            var type = (MessageType)raw[offset++];

            byte[] guidBytes = new byte[16];
            Buffer.BlockCopy(raw, offset, guidBytes, 0, 16);
            offset += 16;
            Guid messageId = new Guid(guidBytes);

            ushort length = (ushort)((raw[offset++] << 8) | raw[offset++]);
            byte reserved = raw[offset++];

            if (length + offset > raw.Length) return;

            lock (_receivedIds)
            {
                if (!_receivedIds.Add(messageId))
                    return;
            }

            // Envoi ACK immédiat
            SendAck(peerId, messageId);

            // Payload
            byte[] payload = new byte[length];
            Buffer.BlockCopy(raw, offset, payload, 0, length);
            OnMessageReceived?.Invoke(peerId, type, payload);
        }

        private static void SendAck(string peerId, Guid messageId)
        {
            byte[] ack = new byte[1 + 16];
            ack[0] = 255; // Type ACK réservé
            Buffer.BlockCopy(messageId.ToByteArray(), 0, ack, 1, 16);
            if (NetworkRegistry.Peers.TryGetValue(peerId, out var peer))
                peer.Send(ack);
        }

        /// Gère la réception d’un ACK
        public static void HandleAckBytes(string peerId, byte[] raw)
        {
            if (raw.Length != 17 || raw[0] != 255) return;

            byte[] guidBytes = new byte[16];
            Buffer.BlockCopy(raw, 1, guidBytes, 0, 16);
            Guid messageId = new Guid(guidBytes);

            if (_pendingAcks.TryRemove(messageId, out var entry))
            {
                entry.Callback?.Invoke();
                OnMessageAcknowledged?.Invoke(entry.PeerId, messageId);
            }
        }

        public static IReadOnlyDictionary<Guid, (string peerId, byte[] envelope)> GetFailedMessages()
            => _failedMessages;
    }
}
