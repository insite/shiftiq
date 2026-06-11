using System;

namespace InSite.Persistence
{
    public class Occupation
    {
        public String JobTitle { get; set; }
        public Guid OccupationIdentifier { get; set; }
        public Int32 OccupationKey { get; set; }
        public String Purpose { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public String Statements { get; set; }
    }
}
