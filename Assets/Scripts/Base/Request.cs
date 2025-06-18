public abstract class Request<T>
{
    /// La valeur obtenue après résolution de la requête.
    public T Result { get; protected set; }

    /// Indique si la requête est terminée (résolue).
    public bool IsFinished { get; protected set; } = false;

    /// Méthode à implémenter : déclenche le traitement de la requête.
    /// Peut être synchrone ou appeler un UI/prompt pour retarder.
    public abstract void Execute();

    protected Request()
    {
        RequestSystem.Enqueue(this);
    }

    /// Appelée pour fournir le résultat de la requête (ex: par le joueur ou un système).
    public void Resolve(T result)
    {
        Result = result;
        IsFinished = true;
    }
}
