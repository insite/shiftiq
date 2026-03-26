using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using InSite.Common.Web;

using Newtonsoft.Json;

namespace InSite.UI.Lobby.Integration.Saml
{
    public partial class Launch : InSite.UI.Layout.Lobby.Controls.SignInBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var secret = ServiceLocator.AppSettings.Security.Saml.ServiceProviderSecret;

            SamlToken.Initialize(secret);

            var token = Context.Request.Cookies[SamlToken.CookieName].Value;

            var user = SamlToken.ValidateToken(token);

            if (user != null)
            {
                LoginUser(user.Email, string.Empty, true, Shift.Constant.AuthenticationSource.Saml);
            }
            else
            {
                HttpResponseHelper.SendHttp401(Response);
            }
        }
    }

    public static class SamlToken
    {
        public const string CookieName = "shuttle.saml.auth.token";
        public const int TokenLifetimeHours = 8;

        private static byte[] _secretKey;

        public static void Initialize(string base64Key)
        {
            if (string.IsNullOrEmpty(base64Key))
            {
                throw new InvalidOperationException(
                    "JWT secret key not configured. Set 'Shuttle:JwtSecret' in appsettings.json.");
            }

            _secretKey = Convert.FromBase64String(base64Key);

            if (_secretKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT secret key must be at least 32 bytes (256 bits). " +
                    "Generate with: Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))");
            }
        }

        public static SamlUser ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var parts = token.Split('.');
            if (parts.Length != 3)
                return null;

            var message = $"{parts[0]}.{parts[1]}";
            var expectedSignature = ComputeSignature(message);

            if (!ConstantTimeEquals(parts[2], expectedSignature))
                return null;

            try
            {
                var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
                var payload = JsonConvert.DeserializeObject<JwtPayload>(payloadJson);

                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (payload.exp < now)
                    return null;

                return new SamlUser
                {
                    NameId = payload.sub,
                    Email = payload.email,
                    FirstName = payload.given_name,
                    LastName = payload.family_name,
                    DisplayName = payload.name,
                    Roles = payload.roles ?? new List<string>()
                };
            }
            catch
            {
                return null;
            }
        }

        private static string ComputeSignature(string message)
        {
            using (var hmac = new HMACSHA256(_secretKey))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                return Base64UrlEncode(hash);
            }
        }

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string text)
        {
            text = text.Replace('-', '+').Replace('_', '/');

            switch (text.Length % 4)
            {
                case 2: text += "=="; break;
                case 3: text += "="; break;
            }

            return Convert.FromBase64String(text);
        }

        private static bool ConstantTimeEquals(string a, string b)
        {
            if (a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }

        private class JwtPayload
        {
            public string sub { get; set; }
            public string email { get; set; }
            public string given_name { get; set; }
            public string family_name { get; set; }
            public string name { get; set; }
            public List<string> roles { get; set; }
            public long iat { get; set; }
            public long exp { get; set; }
            public string iss { get; set; }
        }
    }

    public class SamlUser
    {
        public string NameId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}