using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectPendingPeople : Query<IEnumerable<PendingPersonModel>>, IPendingPersonCriteria
    {
        public Guid? SubmittedBy { get; set; }
        public Guid? OrganizationId { get; set; }

        public string PersonCode { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }

        public DateTimeOffset? SubmittedBefore { get; set; }
        public DateTimeOffset? SubmittedSince { get; set; }
    }
}
