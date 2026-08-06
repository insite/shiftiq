using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountPendingPeople : Query<int>, IPendingPersonCriteria
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
