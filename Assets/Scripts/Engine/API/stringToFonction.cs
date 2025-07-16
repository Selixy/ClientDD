using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using Selixy_Utils;
using RPG_System.Networking;

namespace RPG_System.API
{
    public class stringToFunction
    {
        /// Exécute une commande de la forme :
        ///   [clientID/]API/FunctionName[/arguments]
        /// où arguments peut être un CSV (a,b,c) ou un JSON array ["a","b","c"].
        public bool Execute(string fullCommand, out string output)
        {
            output = "";

            if (string.IsNullOrWhiteSpace(fullCommand))
            {
                output = "Commande vide";
                return false;
            }

            var parts = fullCommand
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int idx = 0;
            string clientId = null;

            // si le premier segment n'est pas "API", on le prend comme clientID
            if (parts.Length > 0
                && !parts[0].Equals("API", StringComparison.OrdinalIgnoreCase))
            {
                clientId = parts[0];
                idx++;
            }

            // on attend "API"
            if (parts.Length - idx < 1
                || !parts[idx].Equals("API", StringComparison.OrdinalIgnoreCase))
            {
                output = "Commande invalide. Utilisez [clientID/]API/nomFonction[/arguments]";
                return false;
            }
            idx++;

            // nom de la fonction
            if (parts.Length - idx < 1)
            {
                output = "Fonction manquante";
                return false;
            }
            string func = parts[idx++];

            // argument optionnel
            string input = parts.Length > idx ? parts[idx] : "";

            // routing vers un autre client si besoin
            if (clientId != null && clientId != User_Info.ID)
                return HandleExternalCommand(clientId, func, input, out output);

            // résolution de la méthode statique dans cette classe
            MethodInfo method = typeof(stringToFunction).GetMethod(
                func,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase
            );
            if (method == null)
            {
                output = $"Fonction inconnue : {func}";
                return false;
            }

            // préparation des arguments selon la signature
            object[] args;
            var parameters = method.GetParameters();

            if (parameters.Length == 1)
            {
                var pt = parameters[0].ParameterType;
                if (pt == typeof(byte[]))
                    args = new object[] { ByteUtils.ToBytes(input) };
                else
                    args = new object[] { ConvertSingle(input, pt) };
            }
            else if (parameters.Length > 1)
            {
                // parse CSV ou JSON array
                string[] tokens;
                if (input.StartsWith("[") && input.EndsWith("]"))
                {
                    var js = new DataContractJsonSerializer(typeof(string[]));
                    using var ms = new MemoryStream(Encoding.UTF8.GetBytes(input));
                    tokens = (string[])js.ReadObject(ms);
                }
                else
                {
                    tokens = input
                        .Split(',')
                        .Select(s => s.Trim())
                        .ToArray();
                }

                if (tokens.Length != parameters.Length)
                {
                    output = $"La fonction attend {parameters.Length} arguments, mais reçu {tokens.Length}.";
                    return false;
                }

                args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    args[i] = ConvertSingle(tokens[i], parameters[i].ParameterType);
            }
            else
            {
                args = Array.Empty<object>();
            }

            // invocation & traitement du retour
            try
            {
                object result = method.Invoke(null, args);

                if (result is byte[] b)
                    output = ByteUtils.ToDebugString(b);
                else if (result is string s)
                    output = s;
                else if (result != null)
                    output = result.ToString();
                else
                    output = "";

                return true;
            }
            catch (TargetInvocationException tie)
            {
                output = $"Erreur : {tie.InnerException?.Message ?? tie.Message}";
                return false;
            }
            catch (Exception ex)
            {
                output = $"Erreur : {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Convertit une chaîne en type primitif, enum ou byte[].
        /// </summary>
        private static object ConvertSingle(string str, Type targetType)
        {
            if (targetType == typeof(string))
                return str;
            if (targetType.IsEnum)
                return Enum.Parse(targetType, str, ignoreCase: true);
            if (targetType == typeof(byte[]))
                return ByteUtils.ToBytes(str);
            return Convert.ChangeType(str, targetType);
        }

        /// Redirige une commande destinée à un autre client.
        protected virtual bool HandleExternalCommand(
            string clientId,
            string func,
            string input,
            out string output)
        {
            output = $"Commande pour un autre ID ({clientId}) — à router vers {func}";
            return false;
        }

        // --- Exemples de fonctions exposées par stringToFunction ---

        public static byte[] Test(bool flag)
        {
            return flag
                ? ByteUtils.ToBytes(2.0f, 6, "Yolo")
                : ByteUtils.ToBytes("nope");
        }

        public static byte[] Test2(float f, int i, string msg)
        {
            return ByteUtils.ToBytes(f * i, msg.Length);
        }
    }
}
