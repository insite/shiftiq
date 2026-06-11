using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountAssessments : Query<int>, IAssessmentCriteria
    {
        public Guid? OrganizationId { get; set; }

        public string FormPublicationStatus { get; set; }
        public DateTimeOffset? FormFirstPublishedSince { get; set; }
        public DateTimeOffset? FormFirstPublishedBefore { get; set; }
    }
}