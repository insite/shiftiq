using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountForms : Query<int>, IFormCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
        public DateTimeOffset? SurveyFormClosedSince { get; set; }
        public DateTimeOffset? SurveyFormClosedBefore { get; set; }
        public DateTimeOffset? SurveyFormLockedSince { get; set; }
        public DateTimeOffset? SurveyFormLockedBefore { get; set; }
        public DateTimeOffset? SurveyFormOpenedSince { get; set; }
        public DateTimeOffset? SurveyFormOpenedBefore { get; set; }
    }
}
