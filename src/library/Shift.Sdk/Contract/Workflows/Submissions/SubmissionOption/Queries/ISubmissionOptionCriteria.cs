using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ISubmissionOptionCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationId { get; set; }
    }
}
