using System;

namespace InSite.Persistence
{
    public class PersonPortalSearchResultItem
    {
        public Guid UserIdentifier { get; set; }
        public string PersonCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? Referred { get; set; }
        public DateTimeOffset? LastAuthenticated { get; set; }
    }
}
