using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Selixy_Utils
{
    public static class ByteUtils
    {
        // Sérialise un T en [lenType|typeName|payload]
        public static byte[] ToBytes<T>(T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            // payload brut
            byte[] payload = SerializeValueToPayload(typeof(T), value);

            // nom complet du type
            string typeName = typeof(T).AssemblyQualifiedName;
            byte[] tnBytes = Encoding.UTF8.GetBytes(typeName);
            byte[] tnLen   = BitConverter.GetBytes(tnBytes.Length);

            // concatène [4 bytes longueur][typeName][payload]
            var result = new byte[4 + tnBytes.Length + payload.Length];
            Buffer.BlockCopy(tnLen,   0, result, 0,               4);
            Buffer.BlockCopy(tnBytes, 0, result, 4,               tnBytes.Length);
            Buffer.BlockCopy(payload, 0, result, 4 + tnBytes.Length, payload.Length);
            return result;
        }

        // Sérialise une suite d'objets en un seul byte[]
        public static byte[] ToBytes(params object[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            var singleDef = typeof(ByteUtils)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m =>
                    m.Name == nameof(ToBytes)
                    && m.IsGenericMethodDefinition
                    && m.GetParameters().Length == 1);

            using var ms = new MemoryStream();
            foreach (var v in values)
            {
                if (v == null) throw new ArgumentNullException(nameof(values));
                var method = singleDef.MakeGenericMethod(v.GetType());
                var part   = (byte[])method.Invoke(null, new object[] { v });
                ms.Write(part, 0, part.Length);
            }
            return ms.ToArray();
        }

        // Désérialise un flux complet en T
        public static T FromBytes<T>(byte[] data)
        {
            return (T)FromBytes(data);
        }

        // Renvoie un JSON array de toutes les valeurs contenues dans le flux
        public static string ToDebugString(byte[] data)
        {
            if (data == null) return "null";

            var values = new List<object>();
            int pos = 0, len = data.Length;

            while (pos < len)
            {
                if (pos + 4 > len) throw new ArgumentException("Flux mal formé");
                int tnLen = BitConverter.ToInt32(data, pos);
                pos += 4;

                if (pos + tnLen > len) throw new ArgumentException("Flux mal formé");
                string typeName = Encoding.UTF8.GetString(data, pos, tnLen);
                pos += tnLen;

                Type t = Type.GetType(typeName, throwOnError: true);
                int payloadLen = ComputePayloadLength(t, len - pos);

                if (pos + payloadLen > len) throw new ArgumentException("Flux mal formé");
                byte[] payload = new byte[payloadLen];
                Buffer.BlockCopy(data, pos, payload, 0, payloadLen);
                pos += payloadLen;

                values.Add(DeserializePayload(t, payload));
            }

            // sérialise la liste en JSON array
            var ser = new DataContractJsonSerializer(typeof(object[]));
            using var outMs = new MemoryStream();
            ser.WriteObject(outMs, values.ToArray());
            return Encoding.UTF8.GetString(outMs.ToArray());
        }

        // --- implémentation interne ---

        // lit le premier objet du flux (non utilisé directement)
        private static object FromBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length < 4) throw new ArgumentException("Données trop courtes");

            int tnLen = BitConverter.ToInt32(data, 0);
            if (data.Length < 4 + tnLen) throw new ArgumentException("Données mal formées");

            string typeName = Encoding.UTF8.GetString(data, 4, tnLen);
            Type t = Type.GetType(typeName, throwOnError: true);

            int offset = 4 + tnLen;
            int payloadLen = data.Length - offset;
            byte[] payload = new byte[payloadLen];
            Buffer.BlockCopy(data, offset, payload, 0, payloadLen);

            return DeserializePayload(t, payload);
        }

        private static int ComputePayloadLength(Type t, int remaining)
        {
            if      (t == typeof(bool))    return 1;
            else if (t == typeof(char))    return 2;
            else if (t == typeof(short))   return 2;
            else if (t == typeof(ushort))  return 2;
            else if (t == typeof(int))     return 4;
            else if (t == typeof(uint))    return 4;
            else if (t == typeof(long))    return 8;
            else if (t == typeof(ulong))   return 8;
            else if (t == typeof(float))   return 4;
            else if (t == typeof(double))  return 8;
            else if (t == typeof(decimal)) return 16;
            else if (t == typeof(Guid))    return 16;
            else if (t == typeof(string))  return remaining;
            else                            return remaining;
        }

        private static byte[] SerializeValueToPayload(Type t, object value)
        {
            if (t == typeof(bool))   return BitConverter.GetBytes((bool)value);
            if (t == typeof(char))   return BitConverter.GetBytes((char)value);
            if (t == typeof(short))  return BitConverter.GetBytes((short)value);
            if (t == typeof(ushort)) return BitConverter.GetBytes((ushort)value);
            if (t == typeof(int))    return BitConverter.GetBytes((int)value);
            if (t == typeof(uint))   return BitConverter.GetBytes((uint)value);
            if (t == typeof(long))   return BitConverter.GetBytes((long)value);
            if (t == typeof(ulong))  return BitConverter.GetBytes((ulong)value);
            if (t == typeof(float))  return BitConverter.GetBytes((float)value);
            if (t == typeof(double)) return BitConverter.GetBytes((double)value);
            if (t == typeof(decimal))
            {
                int[] bits = decimal.GetBits((decimal)value);
                var buf = new byte[16];
                for (int i = 0; i < 4; i++)
                    Buffer.BlockCopy(BitConverter.GetBytes(bits[i]), 0, buf, i * 4, 4);
                return buf;
            }
            if (t == typeof(Guid))   return ((Guid)value).ToByteArray();
            if (t == typeof(string)) return Encoding.UTF8.GetBytes((string)value);

            // fallback JSON pour toute classe
            var js = new DataContractJsonSerializer(t);
            using var ms = new MemoryStream();
            js.WriteObject(ms, value);
            return ms.ToArray();
        }

        private static object DeserializePayload(Type t, byte[] p)
        {
            if (t == typeof(bool))   return BitConverter.ToBoolean(p, 0);
            if (t == typeof(char))   return BitConverter.ToChar(p, 0);
            if (t == typeof(short))  return BitConverter.ToInt16(p, 0);
            if (t == typeof(ushort)) return BitConverter.ToUInt16(p, 0);
            if (t == typeof(int))    return BitConverter.ToInt32(p, 0);
            if (t == typeof(uint))   return BitConverter.ToUInt32(p, 0);
            if (t == typeof(long))   return BitConverter.ToInt64(p, 0);
            if (t == typeof(ulong))  return BitConverter.ToUInt64(p, 0);
            if (t == typeof(float))  return BitConverter.ToSingle(p, 0);
            if (t == typeof(double)) return BitConverter.ToDouble(p, 0);
            if (t == typeof(decimal))
            {
                if (p.Length != 16) throw new ArgumentException("Payload invalide pour decimal");
                int[] bits = new int[4];
                for (int i = 0; i < 4; i++)
                    bits[i] = BitConverter.ToInt32(p, i * 4);
                return new decimal(bits);
            }
            if (t == typeof(Guid))   return new Guid(p);
            if (t == typeof(string)) return Encoding.UTF8.GetString(p);

            // fallback JSON pour toute classe
            var js = new DataContractJsonSerializer(t);
            using var ms = new MemoryStream(p);
            return js.ReadObject(ms);
        }
    }
}

