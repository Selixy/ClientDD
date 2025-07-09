namespace RPG_System.Networking
{
    public class NetworkClient : INetworkModule
    {
        public void Initialize()
        {
            // Connexion au serveur
        }

        public void Shutdown()
        {
            // Déconnexion propre
        }

        public void Send(string targetId, string messageType, object data)
        {
            // Envoi d’un message au serveur uniquement
        }

        public void Broadcast(string messageType, object data)
        {
            // Pas applicable, ou rebascule en Send au serveur
        }
    }
}
