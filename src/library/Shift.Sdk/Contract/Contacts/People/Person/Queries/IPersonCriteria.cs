using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPersonCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? UserId { get; set; }

        string EmailExact { get; set; }
        string EmailLike { get; set; }
        string EventRole { get; set; }
        string FullName { get; set; }
        string PersonCode { get; set; }
        string FirstNameExact { get; set; }
        string LastNameExact { get; set; }
        DateTimeOffset? LastAuthenticatedSince { get; set; }
        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }
        bool? IsApproved { get; set; }
    }
}