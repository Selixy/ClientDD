using System.Collections.Generic;

namespace RPG_System.Engine
{
    public static class ConnectedRegistry
    {
        // Liste des clients connectés (valide uniquement côté serveur)
        public static List<string> Clients { get; private set; } = new();

        // Identifiant unique de ce client
        public static string SelfId { get; private set; }

        // Retourne l'ID du serveur (le premier client de la liste)
        public static string ServerID => Clients.Count > 0 ? Clients[0] : null;

        // Indique si ce client est le serveur
        public static bool IsServer => Clients.Count > 0 && ServerID == SelfId;

        // Constructeur statique : exécuté une seule fois
        static ConnectedRegistry()
        {
            Init_Id();
            // On s'ajoute à sa liste si on est le serveur (cas de départ)
            Clients.Add(SelfId);
        }

        // Initialise l'identifiant du client
        public static void Init_Id()
        {
            // TODO : Remplacer par un vrai identifiant (UUID, nom machine, etc.)
            SelfId = "123";
        }

        // Côté serveur : ajoute un client à la liste
        public static void RegisterClient(string id)
        {
            if (!IsServer) return;

            if (!Clients.Contains(id))
            {
                Clients.Add(id);
                BroadcastUpdatedList();
            }
        }

        // Côté serveur : retire un client de la liste
        public static void UnregisterClient(string id)
        {
            if (!IsServer) return;

            if (Clients.Remove(id))
                BroadcastUpdatedList();
        }

        // Côté client : se connecte à un serveur
        public static void ConnectTo(string Server)
        {

        }

        // Côté client : déconnexion
        public static void Disconnect()
        {
            Clients.Clear();
            Clients.Add(SelfId); // revient à une liste avec seulement soi-même
        }

        // Côté serveur : envoie la liste actualisée à tous les clients
        private static void BroadcastUpdatedList()
        {
            // TODO : envoyer la liste Clients à tous les clients connectés
            // Ex. : Network.SendToAll("ClientListSync", Clients);
        }

        // Côté client : recois la liste actualisée de tous les clients
        public static void ReceiveClientList(List<string> list)
        {
            Clients = list
        }

    }
}
