using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class PersonRenamed : Change
    {
        public Guid ContactIdentifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public PersonRenamed(Guid contact, string firstName, string lastName)
        {
            ContactIdentifier = contact;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
