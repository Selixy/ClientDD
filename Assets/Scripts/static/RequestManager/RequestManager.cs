using System.Collections.Generic;

namespace RPG_System
{
    public static class RequestManager
    {
        private static readonly Dictionary<string, RequestQueue> Queues = new();

        public static RequestQueue GlobalQueue { get; } = new();

        public static RequestQueue GetQueue(string key)
        {
            if (!Queues.ContainsKey(key))
                Queues[key] = new RequestQueue();

            return Queues[key];
        }

        public static void ClearAll()
        {
            Queues.Clear();
        }
    }
}
