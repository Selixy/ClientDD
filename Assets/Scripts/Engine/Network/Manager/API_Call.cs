// Api_Call.cs
using System;
using Newtonsoft.Json.Linq;   // nécessite com.unity.nuget.newtonsoft-json
using Selixy_Utils;           // pour ByteUtils
using RPG_System.API;         // pour PublicAPI
using UnityEngine;            // Debug.LogWarning

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        /// Envoie un appel RPC : [groupId(int), handlerId(int), jsonArgs(string)] sérialisés.
        public static void CallRemoteFunction(
            string peerId,
            Group  group,
            int    handlerId,
            string jsonArgs = "[]",
            Action onAck    = null)
        {
            // ByteUtils.ToBytes prend les trois objets et concatène proprement
            byte[] payload = ByteUtils.ToBytes(
                (int)group,
                handlerId,
                jsonArgs
            );
            SendMessage(peerId, MessageType.ApiCall, payload, onAck);
        }

        /// Réception d’un ApiCall : désérialise, appelle PublicAPI, renvoie la réponse.
        internal static void HandleRemoteFunction(string peerId, byte[] payload)
        {
            // 1) Récupère le JSON array [group, handlerId, jsonArgs]
            string arrJson = ByteUtils.ToDebugString(payload);
            var    arr     = JArray.Parse(arrJson);
            int    groupId   = arr[0].Value<int>();
            int    handlerId = arr[1].Value<int>();
            string jsonArgs  = arr[2].Value<string>();

            // 2) Si ce n’est pas pour moi, on rebroadcaste simplement
            if (peerId != User_Info.ID)
            {
                CallRemoteFunction(peerId, (Group)groupId, handlerId, jsonArgs);
                return;
            }

            // 3) Exécution du handler
            byte[] resultPayload;
            try
            {
                // PublicAPI.Handle attend un byte[] pour ses args
                byte[] argsForApi = ByteUtils.ToBytes(jsonArgs);
                resultPayload     = PublicAPI.Handle(handlerId, argsForApi);
            }
            catch (Exception ex)
            {
                resultPayload = ByteUtils.ToBytes($"Handler error: {ex.Message}");
            }

            // 4) Renvoi de la réponse : on wrappe dans un JSON-array à un seul élément
            string innerJson = ByteUtils.ToDebugString(resultPayload);
            string wrapJson  = $"[{JsonEscape(innerJson)}]";
            CallRemoteFunction(peerId, (Group)groupId, handlerId, wrapJson);
        }

        static string JsonEscape(string s)
            => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    }
}
