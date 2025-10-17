using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ISiteCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }

        string LastChangeType { get; set; }
        string LastChangeUser { get; set; }
        string SiteDomain { get; set; }
        string SiteTitle { get; set; }

        DateTimeOffset? LastChangeTime { get; set; }
    }
}