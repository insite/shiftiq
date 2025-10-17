using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shift.Common
{
    public static class EncryptionHelper
    {
        public static SymmetricAlgorithm GetSymmetricAlgorithm(SymmetricEncryptionAlgorithm cryptoAlg)
        {
            SymmetricAlgorithm symmetricAlg;

            string strSetup = cryptoAlg.ToString();
            string[] parts = strSetup.Split('_');

            switch (parts[0])
            {
                case "AES":
                    symmetricAlg = new AesCryptoServiceProvider();
                    break;
                case "Rijndael":
                    symmetricAlg = new RijndaelManaged();
                    symmetricAlg.BlockSize = int.Parse(parts[3]); // Rijndael is the only one that can set a different block size
                    break;
                case "RC2":
                    symmetricAlg = new RC2CryptoServiceProvider();
                    break;
                default:
                    throw new Exception($"The symmetric encryption algorithm {parts[0]} is not permitted. Please use AES, Rijndael, or RC2.");
            }

            symmetricAlg.KeySize = int.Parse(parts[1]);

            switch (parts[2])
            {
                case "CBC":
                    symmetricAlg.Mode = CipherMode.CBC;
                    break;
                case "ECB":
                    symmetricAlg.Mode = CipherMode.ECB;
                    break;
                case "CFB":
                    symmetricAlg.Mode = CipherMode.CFB;
                    break;
                default:
                    throw new Exception("Invalid CipherMode");
            }

            return symmetricAlg;
        }

        public static PaddingMode GetPaddingMode(PaddingMode paddingModes)
        {
            switch (paddingModes)
            {
                case System.Security.Cryptography.PaddingMode.PKCS7:
                    return System.Security.Cryptography.PaddingMode.PKCS7;

                case System.Security.Cryptography.PaddingMode.None:
                    return System.Security.Cryptography.PaddingMode.None;

                case System.Security.Cryptography.PaddingMode.Zeros:
                    return System.Security.Cryptography.PaddingMode.Zeros;

                case System.Security.Cryptography.PaddingMode.ISO10126:
                    return System.Security.Cryptography.PaddingMode.ISO10126;

                case System.Security.Cryptography.PaddingMode.ANSIX923:
                    return System.Security.Cryptography.PaddingMode.ANSIX923;

                default:
                    throw new Exception("Invalid PaddingMode");
            }
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static byte[] Encode(string key, byte[] salt, Action<Stream> write)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Invalid key");

            var keyHash = ComputeHashSha256(key, salt);

            return Encode(keyHash, write);
        }

        public static byte[] Encode(byte[] keyHash, Action<Stream> write)
        {
            if (keyHash.IsEmpty())
                throw new ArgumentException("Invalid key");

            using (var alg = new RijndaelManaged())
            {
                alg.Key = keyHash;

                using (var encryptor = alg.CreateEncryptor())
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            write(cs);
                        }

                        return ArrayHelper.Combine(alg.IV, ms.ToArray());
                    }
                }
            }
        }

        public static object Decode(byte[] data, int dataOffset, int dataLength, string key, byte[] salt, Func<Stream, object> read)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Invalid key");

            var keyHash = ComputeHashSha256(key, salt);

            return Decode(data, dataOffset, dataLength, keyHash, read);
        }

        public static object Decode(byte[] data, int dataOffset, int dataLength, byte[] keyHash, Func<Stream, object> read)
        {
            if (keyHash.IsEmpty())
                throw new ArgumentException("Invalid key");

            using (var alg = new RijndaelManaged())
            {
                alg.Key = keyHash;
                alg.IV = ArrayHelper.Get(data, dataOffset, 16);

                using (var decryptor = alg.CreateDecryptor())
                {
                    data = ArrayHelper.Get(data, dataOffset + 16, dataLength - 16);

                    using (var ms = new MemoryStream(data))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            return read(cs);
                        }
                    }
                }
            }
        }

        public static byte[] ComputeHashSha256(string value, byte[] salt = null) =>
            ComputeHashSha256(Encoding.UTF8.GetBytes(value), salt);

        public static byte[] ComputeHashSha256(byte[] value, byte[] salt = null)
        {
            if (salt != null && salt.Length > 0)
                value = ArrayHelper.Combine(value, salt);

            using (var hashProvider = new SHA256Managed())
                return hashProvider.ComputeHash(value);
        }

        public static byte[] ComputeHashMd5(string value, byte[] salt = null) =>
            ComputeHashMd5(Encoding.UTF8.GetBytes(value), salt);

        public static byte[] ComputeHashMd5(byte[] value, byte[] salt = null)
        {
            if (salt != null && salt.Length > 0)
                value = ArrayHelper.Combine(value, salt);

            using (var hashProvider = MD5.Create())
                return hashProvider.ComputeHash(value);
        }

        private static readonly RNGCryptoServiceProvider _random = new RNGCryptoServiceProvider();

        public static byte[] GenerateSalt(int size)
        {
            var result = new byte[size];

            _random.GetBytes(result);

            return result;
        }

        public static byte[] EncodeXor(byte[] data, byte[] key)
        {
            var result = new byte[data.Length];

            for (var i = 0; i < data.Length; i++)
                result[i] = (byte)(data[i] ^ key[i % key.Length]);

            return result;
        }
    }
}