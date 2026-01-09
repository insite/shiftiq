using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountSites : Query<int>, ISiteCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }

        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string SiteDomain { get; set; }
        public string SiteTitle { get; set; }

        public DateTimeOffset? LastChangeTime { get; set; }
    }
}