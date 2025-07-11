using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        // ---- Données pour le retry + stock des échecs ----

        private class AckEntry
        {
            public Action  Callback;
            public int     Attempts;
            public string  Envelope;
            public string  PeerId;
        }

        // Pending ACKs : messageId -> AckEntry
        private static readonly ConcurrentDictionary<string, AckEntry> _pendingAcks
            = new();

        // Stocke les messages ayant échoué après 3 tentatives
        // messageId -> (peerId, envelope)
        private static readonly ConcurrentDictionary<string, (string peerId, string envelope)> _failedMessages
            = new();

        // Pour éviter les doublons à la réception
        private static readonly HashSet<string> _receivedIds = new();

        public static event Action<string, string> OnMessageAcknowledged;
        public static event Action<string, string> OnMessageFailed;

        const char   SEP            = '|';
        const string PREFIX_MSG     = "MSG";
        const string PREFIX_ACK     = "ACK";
        const int    MAX_ATTEMPTS   = 3;
        const int    RETRY_DELAY_MS = 300;

        /// Envoie un message fiable avec retry. onAck est invoqué dès l'ACK.
        public static void SendMessage(string peerId, string payload, Action onAck = null)
        {
            var messageId = Guid.NewGuid().ToString();
            var envelope  = string.Join(SEP, PREFIX_MSG, messageId, payload);

            var entry = new AckEntry
            {
                Callback = onAck,
                Attempts = 0,
                Envelope = envelope,
                PeerId   = peerId
            };
            _pendingAcks[messageId] = entry;

            // Lance les essais asynchrones
            _ = RetrySendAsync(messageId);
        }

        private static async Task RetrySendAsync(string messageId)
        {
            if (!_pendingAcks.TryGetValue(messageId, out var entry))
                return;

            while (entry.Attempts < MAX_ATTEMPTS)
            {
                entry.Attempts++;
                // Envoi
                if (NetworkRegistry.Peers.TryGetValue(entry.PeerId, out var peer))
                    peer.Send(entry.Envelope);

                // Attente de l'ACK ou timeout
                await Task.Delay(RETRY_DELAY_MS);

                // Si l'ACK est déjà arrivé, on arrête
                if (!_pendingAcks.ContainsKey(messageId))
                    return;
            }

            // Après MAX_ATTEMPTS sans ACK -> échec
            if (_pendingAcks.TryRemove(messageId, out entry))
            {
                _failedMessages[messageId] = (entry.PeerId, entry.Envelope);
                OnMessageFailed?.Invoke(entry.PeerId, messageId);
            }
        }

        /// Reçoit RAW. Gère MSG/ACK, ignore doublons, renvoie ACK automatique.
        public static void HandleRawMessage(string peerId, string raw)
        {
            var parts = raw.Split(SEP, 3);
            if (parts.Length < 2) return;

            switch (parts[0])
            {
                case PREFIX_MSG:
                    {
                        var id      = parts[1];
                        var payload = parts.Length >= 3 ? parts[2] : "";

                        // Ignore si déjà traité
                        lock (_receivedIds)
                        {
                            if (!_receivedIds.Add(id))
                                return; 
                        }

                        // Renvoie immédiat de l'ACK
                        var ackEnv = string.Join(SEP, PREFIX_ACK, id);
                        if (NetworkRegistry.Peers.TryGetValue(peerId, out var peer))
                            peer.Send(ackEnv);

                        OnMessageReceived?.Invoke(peerId, payload);
                        break;
                    }

                case PREFIX_ACK:
                    {
                        var id = parts[1];
                        // Débloque le retry / callback
                        if (_pendingAcks.TryRemove(id, out var entry))
                        {
                            entry.Callback?.Invoke();
                            OnMessageAcknowledged?.Invoke(entry.PeerId, id);
                        }
                        break;
                    }
            }
        }

        /// Permet d'interroger les messages qui ont échoué.
        public static IReadOnlyDictionary<string, (string peerId, string envelope)> GetFailedMessages()
            => _failedMessages;
    }
}
