namespace RPG_System.Networking
{
    public class Network
    {
        private INetworkModule _module;

        public void StartAsClient()
        {
            _module?.Shutdown();
            _module = new NetworkClient();
            _module.Initialize();
        }

        public void StartAsServer()
        {
            _module?.Shutdown();
            _module = new NetworkServer();
            _module.Initialize();
        }

        public void Shutdown()
        {
            _module?.Shutdown();
            _module = null;
        }

        public void Send(string targetId, string messageType, object data)
        {
            _module?.Send(targetId, messageType, data);
        }

        public void Broadcast(string messageType, object data)
        {
            _module?.Broadcast(messageType, data);
        }
    }
}
