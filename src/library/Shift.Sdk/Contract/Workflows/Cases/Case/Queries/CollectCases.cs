using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectCases : Query<IEnumerable<CaseModel>>, ICaseCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? CaseClosedSince { get; set; }
        public DateTimeOffset? CaseClosedBefore { get; set; }
        public DateTimeOffset? CaseOpenedSince { get; set; }
        public DateTimeOffset? CaseOpenedBefore { get; set; }
        public DateTimeOffset? CaseReportedSince { get; set; }
        public DateTimeOffset? CaseReportedBefore { get; set; }
        public DateTimeOffset? CaseStatusEffectiveSince { get; set; }
        public DateTimeOffset? CaseStatusEffectiveBefore { get; set; }
        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
    }
}
