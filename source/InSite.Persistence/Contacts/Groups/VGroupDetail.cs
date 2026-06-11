using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class VGroupDetail
    {
        public Guid GroupIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? ParentGroupIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public Guid? GroupStatusItemIdentifier { get; set; }

        public Guid? MessageToAdminWhenEventVenueChanged { get; set; }
        public Guid? MessageToAdminWhenMembershipEnded { get; set; }
        public Guid? MessageToAdminWhenMembershipStarted { get; set; }
        public Guid? MessageToUserWhenMembershipEnded { get; set; }
        public Guid? MessageToUserWhenMembershipStarted { get; set; }

        public bool AddNewUsersAutomatically { get; set; }
        public bool AllowSelfSubscription { get; set; }
        public bool OnlyOperatorCanAddUser { get; set; }
        public int? GroupCapacity { get; set; }
        public string GroupCategory { get; set; }
        public string GroupCode { get; set; }
        public DateTimeOffset GroupCreated { get; set; }
        public string GroupType { get; set; }
        public string GroupDescription { get; set; }
        public string GroupEmail { get; set; }
        public string GroupFax { get; set; }
        public string GroupImage { get; set; }
        public string GroupIndustry { get; set; }
        public string GroupIndustryComment { get; set; }
        public string GroupLabel { get; set; }
        public string GroupName { get; set; }
        public string GroupOffice { get; set; }
        public string GroupPhone { get; set; }
        public string GroupRegion { get; set; }
        public string GroupSize { get; set; }
        public string GroupStatus { get; set; }
        public string GroupWebSiteUrl { get; set; }
        public int? LifetimeQuantity { get; set; }
        public string LifetimeUnit { get; set; }
        public string ShippingPreference { get; set; }
        public string SurveyNecessity { get; set; }

        public virtual VOrganization Organization { get; set; }
        public virtual VGroupDetail Parent { get; set; }

        public virtual ICollection<VGroupDetail> Children { get; set; } = new HashSet<VGroupDetail>();
        public virtual ICollection<Person> EmployeePersons { get; set; } = new HashSet<Person>();
        public virtual ICollection<Membership> Memberships { get; set; } = new HashSet<Membership>();
    }
}