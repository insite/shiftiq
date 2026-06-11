using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ISubmissionAnswerCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationId { get; set; }
    }
}
