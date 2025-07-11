using System;

namespace RPG_System.Networking
{
    public static partial class NetworkRegistry
    {
        // Vide l’historique (utile au lancement de nouvelle partie).
        public static void ClearHistory() => ConnectionHistory.Clear();
    }

    public class ConnectionLogEntry
    {
        public string          PeerId;
        public DateTime        Time;
        public ConnectionEvent Event;
    }

    public enum ConnectionEvent
    {
        Connected,
        Disconnected
    }
}
