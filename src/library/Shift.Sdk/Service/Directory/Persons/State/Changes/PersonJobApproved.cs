using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class PersonJobApproved : Change
    {
        public DateTimeOffset? Approved { get; set; }
        public string ApprovedBy { get; set; }

        public PersonJobApproved(DateTimeOffset? approved, string approvedBy)
        {
            Approved = approved;
            ApprovedBy = approvedBy;
        }
    }
}
