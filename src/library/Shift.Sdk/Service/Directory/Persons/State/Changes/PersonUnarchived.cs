using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class PersonUnarchived : Change
    {
        public DateTimeOffset Date { get; set; }

        public PersonUnarchived(DateTimeOffset date)
        {
            Date = date;
        }
    }
}
