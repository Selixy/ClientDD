using System;
using System.Threading.Tasks;
using RPG_System.Networking;

namespace RPG_System.API
{
    public static partial class PrivateAPI
    {
        public static object PingLoopback(string peerId)
        {
            var begin = DateTime.UtcNow;
            var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            void cb(string id, P2PNetwork.MessageType type, byte[] payload)
            {
                if (id == peerId && payload.Length == 1 && payload[0] == 99)
                    done.TrySetResult(true);
            }

            P2PNetwork.OnMessageReceived += cb;
            P2PNetwork.SendMessage(peerId, P2PNetwork.MessageType.ApiCall, new byte[] { 99 });

            var completed = Task.WhenAny(done.Task, Task.Delay(1000)).GetAwaiter().GetResult();
            P2PNetwork.OnMessageReceived -= cb;

            if (completed == done.Task)
                return (DateTime.UtcNow - begin).TotalMilliseconds;
            else
                return "timeout";
        }
    }
}
