using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IStandardCriteria
    {
        QueryFilter Filter { get; set; }

        Guid OrganizationId { get; set; }
        string ContentTitle { get; set; }
        string StandardType { get; set; }
    }
}