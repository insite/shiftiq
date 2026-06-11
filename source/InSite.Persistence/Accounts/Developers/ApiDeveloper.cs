using System;

namespace InSite.Persistence.Integration
{
    public class ApiDeveloper
    {
        public ApiDeveloper(string organization, string name, string email, string secret, DateTimeOffset? expiry)
        {
            Tenant = organization;
            Name = name;
            Email = email;
            Secret = secret;
            Expiry = expiry;
        }

        public string Tenant { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Secret { get; set; }

        public DateTimeOffset? Expiry { get; set; }

        public Domain.Foundations.User User { get; set; }
    }
}
