using System;

namespace InSite.Persistence
{
    public class VOrganizationPersonAddress
    {
        public Guid OrganizationIdentifier { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public int Occurrences { get; set; }
    }
}
