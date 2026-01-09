using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using OtpNet;

namespace Shift.Toolbox
{
    public static class MFAHelpers
    {
        private static int head = 0;
        private static byte[] buffer;
        private const int maxBufferSiz = 7_000;
        private static byte[] GetRandomBytes(int count = 70)
        {
            if (buffer == null)
            {
                buffer = new byte[maxBufferSiz];
                PopulateBuffer(buffer);
            }
            if (head + count >= maxBufferSiz)
            {
                head = 0;
                buffer = new byte[maxBufferSiz];
                PopulateBuffer(buffer);
            }
            var temp = buffer.Skip(head).Take(count).ToArray();
            head += count;
            return temp;
        }
        private static void PopulateBuffer(byte[] buffer)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
            }
        }
        public static (byte[] token, List<string> recovery) GenerateMFATokens()
        {
            var raw = GetRandomBytes();
            var tokens = new List<string>();
            for (int i = 0; i < 12; i++)
            {
                tokens.Add(Base32Encoding.ToString(raw.Skip(i * 4).Take(4).ToArray()).Substring(0, 6));
            }
            return (raw.Skip(60).ToArray(), tokens);
        }
        public static byte[] GenerateMFAToken()
        {
            var raw = GetRandomBytes(10);
            return raw.ToArray();
        }
        public static string ToBase32(this byte[] secret) => Base32Encoding.ToString(secret);

        public static bool VerifyCode(byte[] secret, string code)
        {
            var otp = new OtpNet.Totp(secret);
            var verificationWindow = new VerificationWindow(previous: 1, future: 1);
            return otp.VerifyTotp(code, out var _, verificationWindow);
        }

        public static string GetCode(byte[] secret)
        {
            var otp = new OtpNet.Totp(secret);
            return otp.ComputeTotp();
        }

    }
}
