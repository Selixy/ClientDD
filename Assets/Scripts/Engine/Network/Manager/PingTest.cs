using System;
using System.Threading;
using System.Threading.Tasks;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        // déclenché lorsqu'on reçoit un pong (0x02)
        public static event Action<string> OnPongReceived;

        // envoie 0x01 et attend 0x02 ou timeout
        public static async Task<bool> SendRawPing(
            string peerId,
            int timeoutMs = 1000,
            CancellationToken ct = default)
        {
            var tcs = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            void HandlePong(string id)
            {
                if (id == peerId)
                    tcs.TrySetResult(true);
            }

            OnPongReceived += HandlePong;

            try
            {
                if (!NetworkRegistry.Peers.TryGetValue(peerId, out var peer))
                    return false;

                peer.Send(new byte[] { 0x01 });

                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                linkedCts.CancelAfter(timeoutMs);

                var completed = await Task.WhenAny(
                    tcs.Task,
                    Task.Delay(-1, linkedCts.Token)
                ).ConfigureAwait(false);

                return completed == tcs.Task;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            finally
            {
                OnPongReceived -= HandlePong;
            }
        }
    }
}
