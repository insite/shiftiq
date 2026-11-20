using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Test.Common
{
    public class JsonWebTokenTests
    {
        private const string Secret = "mysecret";
        private const string Issuer = "testIssuer";
        private const string Subject = "testSubject";
        private const string Audience = "testAudience";
        private readonly DateTimeOffset Expiry = DateTimeOffset.UtcNow.AddHours(1);

        private string CreateValidJwt(Dictionary<ClaimName, string> payload)
        {
            var header = new Dictionary<ClaimName, string>
            {
                { ClaimName.Algorithm, "HS256" },
                { ClaimName.Type, "JWT" }
            };

            string headerJson = JsonConvert.SerializeObject(header);
            string payloadJson = JsonConvert.SerializeObject(payload);

            string encodedHeader = EncodeBase64(Encoding.UTF8.GetBytes(headerJson));
            string encodedPayload = EncodeBase64(Encoding.UTF8.GetBytes(payloadJson));
            string signature = CreateSignature(Secret, $"{encodedHeader}.{encodedPayload}");

            return $"{encodedHeader}.{encodedPayload}.{signature}";
        }

        private string EncodeBase64(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private string CreateSignature(string secret, string input)
        {
            var encoding = new UTF8Encoding();
            byte[] secretBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(input);
            using (var hmacsha256 = new HMACSHA256(secretBytes))
            {
                byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);
                return EncodeBase64(hashMessage);
            }
        }

        [Fact]
        public void Constructor_EmptyJwt_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new EncodedJwt(string.Empty));
        }

        [Fact]
        public void Constructor_ValidJwt_SetsPropertiesCorrectly()
        {
            var payload = new Dictionary<ClaimName, string> { { ClaimName.Subject, "Bad Horse" } };
            var jwt = CreateValidJwt(payload);
            var token = new EncodedJwt(jwt);

            Assert.Equal(jwt.Split('.')[0], token.Header);
            Assert.Equal(jwt.Split('.')[1], token.Payload);
            Assert.Equal(jwt.Split('.')[2], token.Signature);

            var encoder = new JwtEncoder();
            var decoded = encoder.Decode(jwt);
            Assert.Equal("Bad Horse", decoded.GetClaimValue(ClaimName.Subject).ToString());
        }

        [Fact]
        public void Token_ReturnsCorrectToken()
        {
            var payload = new Dictionary<ClaimName, string>();
            var jwt = CreateValidJwt(payload);
            var token = new EncodedJwt(jwt);

            Assert.Equal(jwt, token.ToString());
        }

        [Fact]
        public void VerifySignature_ValidSignature_ReturnsTrue()
        {
            var payload = new Dictionary<ClaimName, string>();
            var jwt = CreateValidJwt(payload);
            var token = new EncodedJwt(jwt);
            var encoder = new JwtEncoder();

            Assert.True(encoder.VerifySignature(token.ToString(), Secret));
        }

        [Fact]
        public void VerifySignature_InvalidSignature_ReturnsFalse()
        {
            var payload = new Dictionary<ClaimName, string>();
            var jwt = CreateValidJwt(payload);
            var token = new EncodedJwt(jwt);
            var encoder = new JwtEncoder();

            Assert.False(encoder.VerifySignature(token.ToString(), "invalidsecret"));
        }

        [Fact]
        public void Constructor_ValidParameters_SetsPropertiesCorrectly()
        {
            var payload = new Dictionary<ClaimName, string>();
            var token = new Jwt(payload, Subject, Issuer, Audience, Expiry);

            Assert.Equal(Issuer, token.Issuer);
            Assert.Equal(Subject, token.Subject);
            Assert.Equal(Audience, token.Audience);
            Assert.Equal(Expiry.ToUnixTimeSeconds().ToString(), token.GetClaimValue(ClaimName.Expiry));
        }
    }
}
