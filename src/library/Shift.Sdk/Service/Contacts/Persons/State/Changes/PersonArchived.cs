using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class PersonArchived : Change
    {
        public DateTimeOffset Date { get; set; }

        public PersonArchived(DateTimeOffset date)
        {
            Date = date;
        }
    }
}
