using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class PersonAccessGranted : Change
    {
        public DateTimeOffset Granted { get; set; }
        public string GrantedBy { get; set; }

        public PersonAccessGranted(DateTimeOffset granted, string grantedBy)
        {
            Granted = granted;
            GrantedBy = grantedBy;
        }
    }
}
