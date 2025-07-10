using System;
using Unity.WebRTC;

namespace RPG_System.Networking
{
    [Serializable]
    public class OfferMessage
    {
        public string PeerId;
        public string Sdp;
        public RTCIceCandidateInit[] IceCandidates;
    }

    [Serializable]
    public class AnswerMessage
    {
        public string PeerId;
        public string Sdp;
        public RTCIceCandidateInit[] IceCandidates;
    }

    [Serializable]
    public class IceMessage
    {
        public string PeerId;
        public RTCIceCandidateInit[] IceCandidates;
    }

    public static class SignalMessageCodec
    {
        public static string Encode<T>(T message)
        {
            var json = UnityEngine.JsonUtility.ToJson(message);
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        }

        public static T Decode<T>(string encoded)
        {
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            return UnityEngine.JsonUtility.FromJson<T>(json);
        }
    }
}
