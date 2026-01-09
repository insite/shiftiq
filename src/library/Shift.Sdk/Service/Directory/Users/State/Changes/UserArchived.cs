using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class UserArchived : Change
    {
        public DateTimeOffset Date { get; set; }

        public UserArchived(DateTimeOffset date)
        {
            Date = date;
        }
    }
}
