using System;
using System.Security.Cryptography;

namespace Shift.Common
{
    public interface ISecret
    {
        string Name { get; }
        string Type { get; }
        string Value { get; }

        int? LifetimeLimitMinutes { get; }

        DateTimeOffset Expiry { get; }
    }

    public class Secret : ISecret
    {
        public string Name { get; }
        public string Type { get; }
        public string Value { get; }
        
        public int? LifetimeLimitMinutes { get; }

        public DateTimeOffset Expiry { get; }

        public Secret(string type, string name)
        {
            Type = type;

            Name = name;
            
            Value = CreateValue();
            
            Expiry = DateTime.Today.AddYears(1);
        }

        public static string CreateValue()
        {
            using (var provider = new RNGCryptoServiceProvider())
            {
                var secretKeyBytes = new byte[64];

                provider.GetBytes(secretKeyBytes);
                
                return Convert.ToBase64String(secretKeyBytes);
            }
        }
    }
}