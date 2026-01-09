using System;
using System.IO;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Contract;

namespace Shift.Common
{
    public class CookieSerializationException : Exception
    {
        public CookieSerializationException(string message, Exception inner) : base(message, inner) { }
    }

    public class CookieTokenEncoder
    {
        public string Serialize(CookieToken token, bool encrypt, string secret)
        {
            if (token == null)
                return null;

            var keyHash = GetKeyHash(secret, encrypt);

            return Serialize(token, keyHash);
        }

        private string Serialize(CookieToken token, byte[] hash)
        {
            var encrypt = hash != null;

            var result = JsonConvert.SerializeObject(token, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return encrypt
                ? EncodeBase64(result, hash)
                : EncodeForCookie(result);
        }

        public CookieToken Deserialize(string token, bool encrypt, string secret)
        {
            try
            {
                if (token.IsEmpty())
                    return null;

                var keyHash = GetKeyHash(secret, encrypt);

                return Deserialize(token, keyHash);
            }
            catch (Exception inner)
            {
                var error = "Deserialization of cookie token failed unexpectedly.";
                throw new CookieSerializationException(error, inner);
            }
        }

        private CookieToken Deserialize(string data, byte[] hash)
        {
            var encrypt = hash != null;

            var decoded = encrypt
                ? DecodeBase64(data, hash)
                : DecodeFromCookie(data);

            return JsonConvert.DeserializeObject<CookieToken>(decoded);
        }

        private static byte[] GetKeyHash(string key, bool encrypt)
            => encrypt && key.IsNotEmpty() ? EncryptionHelper.ComputeHashSha256(key, null) : null;

        public static string DecodeFromCookie(string cookieValue)
            => Uri.UnescapeDataString(cookieValue);

        private static string DecodeBase64(string encrypted, byte[] keyHash)
        {
            var decoded = Convert.FromBase64String(encrypted);

            return (string)EncryptionHelper.Decode(decoded, 0, decoded.Length, keyHash, stream =>
            {
                using (var reader = new StreamReader(stream))
                {
                    string decrypted = reader.ReadToEnd();

                    return decrypted;
                }
            });
        }

        public static string EncodeForCookie(string json)
            => Uri.EscapeDataString(json);

        private static string EncodeBase64(string data, byte[] keyHash)
        {
            var encrypted = EncryptionHelper.Encode(keyHash, stream =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(data);
                }
            });

            var encoded = Convert.ToBase64String(encrypted);

            return encoded;
        }
    }
}