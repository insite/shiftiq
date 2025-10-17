using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Events.Read
{
    public class QEventAttendee
    {
        public Guid EventIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string AttendeeRole { get; set; }

        public DateTimeOffset? Assigned { get; set; }
        
        public virtual QEvent Event { get; set; }
        public virtual VPerson Person { get; set; }

        public string UserCode => Person?.PersonCode;
        public string UserEmail => Person?.User?.UserEmail;
        public string UserFullName => Person?.User?.UserFullName;
        public string UserPhone => Person?.UserPhone;
    }
}
