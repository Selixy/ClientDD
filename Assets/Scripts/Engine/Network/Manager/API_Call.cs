using System;
using System.IO;
using RPG_System.API;

namespace RPG_System.Networking
{
    public static partial class P2PNetwork
    {
        public static void CallRemoteFunction(string peerId, Group group, int id, byte[] input = null, Action onAck = null)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            writer.Write((int)group); // codé sur 4 octets
            writer.Write(id);         // codé sur 4 octets

            if (input != null)
                writer.Write(input);

            SendMessage(peerId, MessageType.ApiCall, ms.ToArray(), onAck);
        }

        internal static void HandleRemoteFunction(string peerId, byte[] payload)
        {
            if (payload.Length < 8)
            {
                UnityEngine.Debug.LogWarning("API payload trop court");
                return;
            }

            //  Si ce n’est pas notre message, on redirige
            if (peerId != User_Info.ID)
            {
                // Re-broadcast vers le vrai destinataire
                SendMessage(peerId, MessageType.ApiCall, payload);
                return;
            }

            int group = BitConverter.ToInt32(payload, 0);
            int id    = BitConverter.ToInt32(payload, 4);
            byte[] input = null;

            if (payload.Length > 8)
            {
                input = new byte[payload.Length - 8];
                Buffer.BlockCopy(payload, 8, input, 0, input.Length);
            }

            switch ((Group)group)
            {
                case Group.ClientAPI:
                    ClientAPI.Handle(id, input);
                    break;
                case Group.ClientAPI_Network:
                    ClientAPI.HandleNetwork(id, input);
                    break;
                case Group.ClientAPI_Get:
                    ClientAPI_Get.Handle(id, input);
                    break;
                case Group.ServerAPI:
                    ServerAPI.Handle(id, input);
                    break;
                case Group.ServerAPI_Network:
                    ServerAPI.HandleNetwork(id, input);
                    break;
                case Group.ServerAPI_Get:
                    ServerAPI_Get.Handle(id, input);
                    break;
                default:
                    UnityEngine.Debug.LogWarning($"Groupe API inconnu : {group}");
                    break;
            }
        }
    }
}
