namespace RPG_System.Networking
{
    public interface INetworkModule
    {
        void Initialize();
        void Shutdown();
        void Send(string targetId, string messageType, object data);
        void Broadcast(string messageType, object data);
    }
}
