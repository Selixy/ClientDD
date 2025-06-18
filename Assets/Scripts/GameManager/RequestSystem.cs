using System.Collections.Generic;

public static class RequestSystem
{
    private static readonly Queue<Request<object>> _queue = new();
    private static Request<object> _current;

    public static void Enqueue<T>(Request<T> request)
    {
        // Boxé en object pour stockage homogène
        _queue.Enqueue(request as Request<object>);
    }

    public static void Tick()
    {
        if (_current == null && _queue.Count > 0)
        {
            _current = _queue.Dequeue();
            _current.Execute();
        }

        if (_current != null && _current.IsFinished)
        {
            _current = null;
        }
    }

    public static bool IsIdle => _current == null && _queue.Count == 0;
}