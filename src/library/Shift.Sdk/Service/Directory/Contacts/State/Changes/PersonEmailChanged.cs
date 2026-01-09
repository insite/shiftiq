using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class PersonEmailChanged : Change
    {
        public Guid ContactIdentifier { get; set; }
        public string Email { get; set; }

        public PersonEmailChanged(Guid contact, string email)
        {
            ContactIdentifier = contact;
            Email = email;
        }
    }
}
