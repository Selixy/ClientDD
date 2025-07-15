using System;
using System.Reflection;
using UnityEngine;

namespace RPG_System.API
{
    public class stringToFunction
    {
        /// Exécute une commande du format :
        ///   [id/]group[_subgroup] "/" fonction ["/" input]
        /// où subgroup peut être "get" ou "net".
        public bool Execute(string fullCommand, out string output)
        {
            output = "";

            if (string.IsNullOrWhiteSpace(fullCommand))
            {
                output = "<color=red>Commande vide</color>";
                return false;
            }

            // 1) Split, on enlève les segments vides
            var parts = fullCommand
                .Split(new[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
            int idx = 0;
            string id = null;

            // 2) Détecter un ID si le premier segment n'est pas un groupe connu
            if (parts.Length > 0 && !IsKnownGroup(parts[0]))
            {
                // Only treat as ID if next segment looks like a group or subgroup
                if (parts.Length >= 2 && 
                   (IsKnownGroup(parts[1]) || IsKnownGroup(parts[0] + "_" + parts[1])))
                {
                    id = parts[0];
                    idx = 1;
                }
            }

            // 3) Déterminer groupKey (groupe ou groupe_sousgroupe)
            string groupKey;
            if (parts.Length - idx >= 2 
                && IsKnownGroup(parts[idx] + "_" + parts[idx+1]))
            {
                // on a un sous-groupe via slash
                groupKey = (parts[idx] + "_" + parts[idx+1]).ToLowerInvariant();
                idx += 2;
            }
            else if (parts.Length - idx >= 1 && IsKnownGroup(parts[idx]))
            {
                // groupe simple ou underscore
                groupKey = parts[idx].ToLowerInvariant();
                idx += 1;
            }
            else
            {
                output = "<color=red>Groupe invalide. " +
                         "Attendu : group[/subgroup]</color>";
                return false;
            }

            // 4) Récupérer nom de la fonction
            if (parts.Length - idx < 1)
            {
                output = "<color=red>Fonction manquante</color>";
                return false;
            }
            string func = parts[idx++];
            
            // 5) Récupérer l'input optionnel
            string input = parts.Length > idx
                         ? parts[idx]
                         : "";

            // 6) Si ID spécifié et différent, router
            if (id != null && id != RPG_System.Networking.User_Info.ID)
                return HandleExternalCommand(id, groupKey, func, input, out output);

            // 7) Résoudre le type à partir du groupKey
            Type target = groupKey switch
            {
                "client"      => typeof(ClientAPI),
                "client_get"  => typeof(ClientAPI_Get),
                "client_net"  => typeof(ClientAPI),
                "server"      => typeof(ServerAPI),
                "server_get"  => typeof(ServerAPI_Get),
                "server_net"  => typeof(ServerAPI),
                _             => null
            };
            if (target == null)
            {
                output = $"<color=red>Groupe inconnu : {groupKey}</color>";
                return false;
            }

            // 8) Chercher la méthode statique (ignore case)
            MethodInfo method = target.GetMethod(
                func,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase
            );
            if (method == null)
            {
                output = $"<color=red>Fonction inconnue : {func} dans {groupKey}</color>";
                return false;
            }

            // 9) Préparer les paramètres (0 ou 1 string)
            object[] args = method.GetParameters().Length == 0
                ? null
                : new object[] { input };

            // 10) Invocation
            try
            {
                object result = method.Invoke(null, args);
                output = result?.ToString() ?? "<color=yellow>Pas de retour</color>";
                return true;
            }
            catch (Exception ex)
            {
                output = $"<color=red>Erreur : {ex.InnerException?.Message ?? ex.Message}</color>";
                return false;
            }
        }

        /// Un groupe connu peut être "client", "server",
        /// ou les formes "client_get", "server_net", etc.
        private bool IsKnownGroup(string g)
        {
            switch (g.ToLowerInvariant())
            {
                case "client":
                case "client_get":
                case "client_net":
                case "server":
                case "server_get":
                case "server_net":
                    return true;
                default:
                    return false;
            }
        }

        /// Override pour router vers un autre ID (multi-instance).
        protected virtual bool HandleExternalCommand(
            string id,
            string groupKey,
            string func,
            string input,
            out string output)
        {
            output = $"<color=yellow>Commande pour un autre ID ({id}) — à router</color>";
            return false;
        }
    }
}
