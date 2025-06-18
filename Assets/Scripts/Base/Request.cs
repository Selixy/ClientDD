public abstract class Request<T>
{
    public T Result { get; protected set; }
    public bool IsFinished { get; protected set; } = false;

    public event System.Action<T> OnResolved;

    protected Request()
    {
        RequestSystem.Enqueue(this);
    }

    public abstract void Execute();

    public void Resolve(T result)
    {
        Result = result;
        IsFinished = true;
        OnResolved?.Invoke(result);
    }
}
