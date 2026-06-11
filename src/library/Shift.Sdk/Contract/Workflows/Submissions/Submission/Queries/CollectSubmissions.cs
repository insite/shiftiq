using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectSubmissions : Query<IEnumerable<SubmissionModel>>, ISubmissionCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }

        public DateTimeOffset? ResponseSessionCompletedSince { get; set; }
        public DateTimeOffset? ResponseSessionCompletedBefore { get; set; }

        public DateTimeOffset? ResponseSessionCreatedSince { get; set; }
        public DateTimeOffset? ResponseSessionCreatedBefore { get; set; }

        public DateTimeOffset? ResponseSessionStartedSince { get; set; }
        public DateTimeOffset? ResponseSessionStartedBefore { get; set; }
    }
}
