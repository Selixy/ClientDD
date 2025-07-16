using System;
using Newtonsoft.Json.Linq;
using Selixy_Utils;
using RPG_System.API;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        // envoie un RPC : [group,int][handlerId,int][args,object[]]
        public static void CallRemoteFunction(
            string peerId,
            Group  group,
            int    handlerId,
            object[] args,
            Action  onAck = null)
        {
            var payload = ByteUtils.ToBytes((int)group, handlerId, args);
            SendMessage(peerId, MessageType.ApiCall, payload, onAck);
        }

        // traite un RPC reçu, exécute PublicAPI, renvoie la réponse
        internal static void HandleRemoteFunction(string peerId, byte[] payload)
        {
            var debug = ByteUtils.ToDebugString(payload);
            var arr   = JArray.Parse(debug);

            int    groupId   = arr[0].Value<int>();
            int    handlerId = arr[1].Value<int>();
            var    args      = arr[2].ToObject<object[]>();

            if (peerId != User_Info.ID)
            {
                CallRemoteFunction(peerId, (Group)groupId, handlerId, args);
                return;
            }

            byte[] resultPayload;
            try
            {
                var apiArgsBytes = ByteUtils.ToBytes(args);
                resultPayload    = PublicAPI.Handle(handlerId, apiArgsBytes);
            }
            catch (Exception ex)
            {
                resultPayload = ByteUtils.ToBytes($"Handler error: {ex.Message}");
            }

            var responseArgs = new object[] { resultPayload };
            CallRemoteFunction(peerId, (Group)groupId, handlerId, responseArgs);
        }
    }
}
