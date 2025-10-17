using System;

namespace InSite.Application.Contacts.Read
{
    public class VDevPerson
    {
        public Guid OrganizationIdentifier { get; set; }
        public string OrganizationCode { get; set; }

        public Guid UserIdentifier { get; set; }
        public string Email { get; set; }

        public Guid? PersonIdentifier { get; set; }
        public DateTimeOffset? UserAccessGranted { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsOperator { get; set; }
    }
}
