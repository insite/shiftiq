using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchRegistrationDetails : Query<IEnumerable<RegistrationDetailsMatch>>, IRegistrationDetailsCriteria
    {
        public Guid? OrganizationId { get; set; }

        public string ApprovalStatus { get; set; }

        public string EventTitle { get; set; }

        public DateTimeOffset? EventScheduledStartSince { get; set; }
        public DateTimeOffset? EventScheduledStartBefore { get; set; }
        public DateTimeOffset? EventScheduledEndSince { get; set; }
        public DateTimeOffset? EventScheduledEndBefore { get; set; }
    }
}
