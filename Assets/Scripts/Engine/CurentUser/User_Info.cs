using System;
using System.Text;

namespace RPG_System.Networking
{
    public class User_Info
    {
        private static readonly Random _rng = new Random();
        private string _tagID;

        public string Pseudo { get; set; }
        public string TagID
        {
            get
            {
                if (string.IsNullOrEmpty(_tagID) || _tagID.Length != 8)
                    _tagID = GenerateRandomString(8);
                return _tagID;
            }
            private set => _tagID = value;
        }

        public string ID => Pseudo + TagID;

        public float ComputationScore => PC_Info.ComputationScore;


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