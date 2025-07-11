using System;
using System.Text;

namespace RPG_System.Networking
{
    public static class User_Info
    {
        private static readonly Random _rng = new Random();
        private static string _tagID;

        public static string Pseudo { get; set; }
        public static string TagID
        {
            get
            {
                if (string.IsNullOrEmpty(_tagID) || _tagID.Length != 8)
                    _tagID = GenerateRandomString(8);
                return _tagID;
            }
            private set => _tagID = value;
        }

        public static string ID => Pseudo + TagID;

        public static float ComputationScore => PC_Info.ComputationScore;


        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                sb.Append(chars[_rng.Next(chars.Length)]);
            return sb.ToString();
        }
    }
}