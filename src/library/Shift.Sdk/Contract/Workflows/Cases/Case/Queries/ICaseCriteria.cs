using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? CaseClosedSince { get; set; }
        DateTimeOffset? CaseClosedBefore { get; set; }
        DateTimeOffset? CaseOpenedSince { get; set; }
        DateTimeOffset? CaseOpenedBefore { get; set; }
        DateTimeOffset? CaseReportedSince { get; set; }
        DateTimeOffset? CaseReportedBefore { get; set; }
        DateTimeOffset? CaseStatusEffectiveSince { get; set; }
        DateTimeOffset? CaseStatusEffectiveBefore { get; set; }
        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
