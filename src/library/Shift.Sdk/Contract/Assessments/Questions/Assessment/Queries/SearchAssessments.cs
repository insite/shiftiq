using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchAssessments : Query<IEnumerable<AssessmentMatch>>, IAssessmentCriteria
    {
        public Guid? OrganizationId { get; set; }

        public string FormPublicationStatus { get; set; }
        public DateTimeOffset? FormFirstPublishedSince { get; set; }
        public DateTimeOffset? FormFirstPublishedBefore { get; set; }
    }
}