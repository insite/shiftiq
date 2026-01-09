using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class UserUnarchived : Change
    {
        public DateTimeOffset Date { get; set; }

        public UserUnarchived(DateTimeOffset date)
        {
            Date = date;
        }
    }
}
