using System;
using System.Collections.Generic;

using InSite.Application.Invoices.Read;
using InSite.Application.Organizations.Read;
using InSite.Application.Records.Read;
using InSite.Application.Surveys.Read;

namespace InSite.Application.Contacts.Read
{
    public class QGroup
    {
        public Guid GroupIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? ParentGroupIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public Guid? MembershipProductIdentifier { get; set; }
        public Guid? GroupStatusItemIdentifier { get; set; }

        public Guid? MessageToAdminWhenEventVenueChanged { get; set; }
        public Guid? MessageToAdminWhenMembershipEnded { get; set; }
        public Guid? MessageToAdminWhenMembershipStarted { get; set; }

        public Guid? MessageToUserWhenMembershipEnded { get; set; }
        public Guid? MessageToUserWhenMembershipStarted { get; set; }

        public bool AddNewUsersAutomatically { get; set; }
        public bool AllowSelfSubscription { get; set; }
        public bool AllowJoinGroupUsingLink { get; set; }
        public bool OnlyOperatorCanAddUser { get; set; }

        public int? GroupCapacity { get; set; }
        public string GroupCategory { get; set; }
        public string GroupCode { get; set; }
        public DateTimeOffset GroupCreated { get; set; }
        public string GroupType { get; set; }
        public string GroupDescription { get; set; }
        public string GroupFax { get; set; }
        public string GroupEmail { get; set; }
        public string GroupImage { get; set; }
        public string GroupIndustry { get; set; }
        public string GroupIndustryComment { get; set; }
        public string GroupLabel { get; set; }
        public string GroupName { get; set; }
        public string GroupOffice { get; set; }
        public string GroupPhone { get; set; }
        public string GroupRegion { get; set; }
        public string GroupSize { get; set; }
        public string GroupWebSiteUrl { get; set; }
        public string ShippingPreference { get; set; }
        public string SurveyNecessity { get; set; }
        public string SocialMediaUrls { get; set; }

        public DateTimeOffset? GroupExpiry { get; set; }
        public int? LifetimeQuantity { get; set; }
        public string LifetimeUnit { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }

        public virtual QGroup Parent { get; set; }
        public virtual QOrganization Organization { get; set; }
        public virtual TProduct MembershipProduct { get; set; }
        public virtual QSurveyForm SurveyForm { get; set; }

        public virtual ICollection<QGroup> Children { get; set; } = new HashSet<QGroup>();
        public virtual ICollection<QGroupAddress> Addresses { get; set; } = new HashSet<QGroupAddress>();
        public virtual ICollection<QGroupConnection> ConnectionChildren { get; set; } = new HashSet<QGroupConnection>();
        public virtual ICollection<QGroupConnection> ConnectionParents { get; set; } = new HashSet<QGroupConnection>();
        public virtual ICollection<TGroupPermission> GroupPermissions { get; set; } = new HashSet<TGroupPermission>();
        public virtual ICollection<VEventGroupPermission> EventGroupPermissions { get; set; } = new HashSet<VEventGroupPermission>();
        public virtual ICollection<QCredential> Credentials { get; set; } = new HashSet<QCredential>();
        public virtual ICollection<VMembership> VMemberships { get; set; } = new HashSet<VMembership>();
        public virtual ICollection<QPerson> EmployeePersons { get; set; } = new HashSet<QPerson>();
        public virtual ICollection<QMembership> QMemberships { get; set; } = new HashSet<QMembership>();
        public virtual ICollection<QJournalSetupGroup> JournalSetupGroups { get; set; } = new HashSet<QJournalSetupGroup>();
        public virtual ICollection<TProgramGroupEnrollment> ProgramGroupEnrollments { get; set; } = new HashSet<TProgramGroupEnrollment>();
    }
}
