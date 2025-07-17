using System;
using System.Threading.Tasks;
using RPG_System.Networking;

namespace RPG_System.API
{
    public static partial class PrivateAPI
    {
        public static (bool success, int time) Ping(string peerId)
        {
            return (false, 1000);
        }
    }
}
