using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPendingPersonCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? SubmittedBy { get; set; }

        string PersonCode { get; set; }
        string UserEmail { get; set; }
        string UserFirstName { get; set; }
        string UserLastName { get; set; }

        DateTimeOffset? SubmittedBefore { get; set; }
        DateTimeOffset? SubmittedSince { get; set; }
    }
}
