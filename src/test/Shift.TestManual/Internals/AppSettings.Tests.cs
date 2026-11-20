using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Test.Internal
{
    public class AppSettingsTests
    {
        private const string Base64KeyIV = "c3VwZXJsb29vb29vb29vb29vb29vb29vb29vb29vb29vb29vb29vb29vb29vb29vb29vb29vb29vb29vb29uZ2tleQ==";

        [Fact]
        public void AppSetings_Decrypt_Success()
        {
            var (json, encrypted) = EncryptAppSettings();
            AppSettings settings;

            using (var stream = new MemoryStream(encrypted))
                settings = AppSettingsFactory.DecryptAppSettings(stream, Base64KeyIV);

            var json2 = JsonConvert.SerializeObject(settings);

            Assert.Equal(json, json2);
        }

        private static (string, byte[]) EncryptAppSettings()
        {
            var settings = AppSettingsFactory.Create();
            var json = JsonConvert.SerializeObject(settings);
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            var keyIV = Convert.FromBase64String(Base64KeyIV);

            var key = new byte[32];
            Array.Copy(keyIV, key, 32);

            var iv = new byte[16];
            Array.Copy(keyIV, 32, iv, 0, 16);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {

                            cryptoStream.Write(jsonBytes, 0, jsonBytes.Length);
                            cryptoStream.FlushFinalBlock();

                            return (json, memoryStream.ToArray());
                        }
                    }
                }
            }
        }
    }
}
