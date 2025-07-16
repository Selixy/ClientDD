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

            if (parts.Length > 0 && !parts[0].Equals("API", StringComparison.OrdinalIgnoreCase))
            {
                clientId = parts[0];
                idx++;
            }

            if (parts.Length - idx < 1 || !parts[idx].Equals("API", StringComparison.OrdinalIgnoreCase))
            {
                output = "Commande invalide. Utilisez [clientID/]API/nomFonction[/arguments]";
                return false;
            }
            idx++;

            if (parts.Length - idx < 1)
            {
                output = "Fonction manquante";
                return false;
            }
            string func = parts[idx++];

            string input = parts.Length > idx ? parts[idx] : "";

            if (clientId != null && clientId != User_Info.ID)
                return HandleExternalCommand(clientId, func, input, out output);

            // 🔁 Nouvelle logique : cherche dans PublicAPI
            MethodInfo method = typeof(PublicAPI).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m => m.Name.Equals(func, StringComparison.OrdinalIgnoreCase)
                                  && m.ReturnType == typeof(byte[]));

            if (method == null)
            {
                output = $"Fonction inconnue : {func}";
                return false;
            }

            object[] args;
            var parameters = method.GetParameters();

            if (parameters.Length == 1)
            {
                var pt = parameters[0].ParameterType;
                args = new object[] { ConvertSingle(input, pt) };
            }
            else if (parameters.Length > 1)
            {
                string[] tokens;
                if (input.StartsWith("[") && input.EndsWith("]"))
                {
                    var js = new DataContractJsonSerializer(typeof(string[]));
                    using var ms = new MemoryStream(Encoding.UTF8.GetBytes(input));
                    tokens = (string[])js.ReadObject(ms);
                }
                else
                {
                    tokens = input.Split(',').Select(s => s.Trim()).ToArray();
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

            try
            {
                object result = method.Invoke(null, args);
                output = result is byte[] b ? ByteUtils.ToDebugString(b) : result?.ToString() ?? "";
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

        protected virtual bool HandleExternalCommand(
            string clientId,
            string func,
            string input,
            out string output)
        {
            output = $"Commande pour un autre ID ({clientId}) — à router vers {func}";
            return false;
        }
    }
}
