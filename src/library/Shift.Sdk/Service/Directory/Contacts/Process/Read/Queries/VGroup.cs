using System;
using System.Collections.Generic;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;

namespace InSite.Application.Contacts.Read
{
    public class VGroup
    {
        public Guid GroupIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? GroupStatusItemIdentifier { get; set; }

        public Guid? MessageToAdminWhenEventVenueChanged { get; set; }

        public string GroupCode { get; set; }
        public string GroupEmail { get; set; }
        public string GroupName { get; set; }
        public string GroupOffice { get; set; }
        public string GroupPhone { get; set; }
        public string GroupType { get; set; }
        public string ParentGroupName { get; set; }
        public int? GroupSize { get; set; }
        public Guid? ParentGroupIdentifier { get; set; }
        public string GroupStatus { get; set; }
        public string GroupRegion { get; set; }

        public virtual ICollection<QEvent> VenueLocationEvents { get; set; } = new HashSet<QEvent>();
        public virtual ICollection<QEvent> VenueOfficeEvents { get; set; } = new HashSet<QEvent>();
        public virtual ICollection<QRegistration> EmployerRegistrations { get; set; } = new HashSet<QRegistration>();
        public virtual ICollection<QRegistration> CustomerRegistrations { get; set; } = new HashSet<QRegistration>();
    }
}