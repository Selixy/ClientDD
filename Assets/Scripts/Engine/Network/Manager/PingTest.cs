using System;
using System.Threading;
using System.Threading.Tasks;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        public static event Action<string> OnPongReceived;
        
        public static async Task<bool> SendRawPing(string peerId, int timeoutMs = 1000)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            void OnPong(string id)
            {
                if (id == peerId)
                    tcs.TrySetResult(true);
            }

            OnPongReceived += OnPong;

            if (NetworkRegistry.Peers.TryGetValue(peerId, out var peer))
                peer.Send(new byte[] { 0x01 });
            else
            {
                OnPongReceived -= OnPong;
                return false;
            }

            var delayTask = Task.Delay(timeoutMs);
            var finished = await Task.WhenAny(tcs.Task, delayTask);

            OnPongReceived -= OnPong;

            return finished == tcs.Task;
        }
    }
}
