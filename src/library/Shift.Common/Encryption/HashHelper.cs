using System.Security.Cryptography;
using System.Text;

namespace Shift.Common
{
    public static class HashHelper
    {
        public static string MD5(string str)
        {
            var encoder = Encoding.Unicode.GetEncoder();

            var unicodeText = new byte[str.Length * 2];

            encoder.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

            byte[] hash;
            using (MD5 md5 = new MD5CryptoServiceProvider())
                hash = md5.ComputeHash(unicodeText);

            return StringHelper.ByteArrayToHex(hash);
        }
    }
}
