using System;

namespace InSite.Persistence
{
    public class DuplicateEmail
    {
        public Int32? DuplicateCount { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public String UserEmail { get; set; }
        public String UserIdentifiers { get; set; }
    }
}
