using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFormQuestionCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationId { get; set; }
    }
}
