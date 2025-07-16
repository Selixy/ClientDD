using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;    // nécessite com.unity.nuget.newtonsoft-json
using Selixy_Utils;

namespace RPG_System.Networking.Public
{
    // 1) Attribut RPC (déclaré une seule fois)
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RpcHandlerAttribute : Attribute
    {
        public int Id { get; }
        public RpcHandlerAttribute(int id) => Id = id;
    }

    // 2) Handle centralisé
    public static partial class PublicAPI
    {
        static readonly Dictionary<int,MethodInfo> _handlers;

        static PublicAPI()
        {
            _handlers = typeof(PublicAPI)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Select(m => (m, attr: m.GetCustomAttribute<RpcHandlerAttribute>()))
                .Where(x => x.attr != null)
                .ToDictionary(x => x.attr.Id, x => x.m);
        }

        public static byte[] Handle(int id, byte[] payload)
        {
            if (!_handlers.TryGetValue(id, out var method))
                return ByteUtils.ToBytes($"Unknown request id={id}");

            try
            {
                var args   = DeserializeArguments(method, payload);
                var result = method.Invoke(null, args);
                return SerializeResult(result);
            }
            catch (Exception ex)
            {
                return ByteUtils.ToBytes($"Error in handler {id}: {ex.Message}");
            }
        }

        static object[] DeserializeArguments(MethodInfo method, byte[] payload)
        {
            string json = ByteUtils.FromBytes<string>(payload);
            var  arr    = JArray.Parse(json);
            var  parms  = method.GetParameters();

            if (arr.Count != parms.Length)
                throw new ArgumentException(
                  $"Handler '{method.Name}' expects {parms.Length} args, got {arr.Count}"
                );

            var args = new object[parms.Length];
            for (int i = 0; i < parms.Length; i++)
                args[i] = arr[i].ToObject(parms[i].ParameterType);

            return args;
        }

        static byte[] SerializeResult(object result)
        {
            return result switch
            {
                byte[] b => b,
                string s => ByteUtils.ToBytes(s),
                null     => ByteUtils.ToBytes(string.Empty),
                _        => ByteUtils.ToBytes(result.ToString())
            };
        }
    }
}
