namespace RPG_System.Networking
{
    public class NetworkServer : INetworkModule
    {
        public void Initialize()
        {
            // Initialiser les sockets, accepter les connexions
        }

        public void Shutdown()
        {
            // Fermer toutes les connexions
        }

        public void Send(string targetId, string messageType, object data)
        {
            // Envoi direct à un client
        }

        public void Broadcast(string messageType, object data)
        {
            // Diffuser à tous les clients connectés
        }
    }
}
