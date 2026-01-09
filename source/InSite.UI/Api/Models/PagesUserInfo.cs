using System;
using System.Web;

using InSite.Persistence;

using Newtonsoft.Json;

namespace InSite.Api.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PagesUserInfo
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; private set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; private set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; private set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; private set; }

        public static PagesUserInfo Get(Guid organizationIdentifier)
        {
            var user = HttpContext.Current?.User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
                return null;

            var token = CookieTokenModule.Current;
            if (token == null || string.IsNullOrEmpty(token.UserEmail) || !string.Equals(token.UserEmail, user.Identity.Name, StringComparison.OrdinalIgnoreCase))
                return null;

            var dbPerson = PersonCriteria.BindFirst(
                x => new
                {
                    UserIdentifier = x.UserIdentifier,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    Phone = x.Phone
                },
                new PersonFilter
                {
                    OrganizationIdentifier = organizationIdentifier,
                    EmailExact = token.UserEmail,
                    IsApproved = true
                });
            if (dbPerson == null)
                return null;

            return new PagesUserInfo
            {
                FirstName = dbPerson.FirstName,
                LastName = dbPerson.LastName,
                Email = dbPerson.Email,
                Phone = dbPerson.Phone
            };
        }
    }
}