using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFormCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }
        DateTimeOffset? SurveyFormClosedSince { get; set; }
        DateTimeOffset? SurveyFormClosedBefore { get; set; }
        DateTimeOffset? SurveyFormLockedSince { get; set; }
        DateTimeOffset? SurveyFormLockedBefore { get; set; }
        DateTimeOffset? SurveyFormOpenedSince { get; set; }
        DateTimeOffset? SurveyFormOpenedBefore { get; set; }
    }
}
