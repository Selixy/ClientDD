using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPG_System
{
    public class RequestQueue
    {
        private readonly Queue<Func<Task>> _queue = new();
        private bool _isProcessing = false;

        public void Enqueue(Func<Task> step)
        {
            _queue.Enqueue(step);
            if (!_isProcessing)
                _ = ProcessQueue();
        }

        private async Task ProcessQueue()
        {
            _isProcessing = true;

            while (_queue.Count > 0)
            {
                var current = _queue.Dequeue();
                await current();
            }

            _isProcessing = false;
        }
    }
}
