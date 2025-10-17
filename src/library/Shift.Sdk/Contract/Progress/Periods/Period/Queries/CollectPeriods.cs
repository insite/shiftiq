using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectPeriods : Query<IEnumerable<PeriodModel>>, IPeriodCriteria
    {
        public Guid OrganizationId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset? EndBefore { get; set; }
        public DateTimeOffset? EndSince { get; set; }
        public DateTimeOffset? StartBefore { get; set; }
        public DateTimeOffset? StartSince { get; set; }
    }
}