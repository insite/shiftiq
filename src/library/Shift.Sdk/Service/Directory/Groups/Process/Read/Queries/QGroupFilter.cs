using System;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    [Serializable]
    public class QGroupFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid? ParentGroupIdentifier { get; set; }
        public Guid? ConnectParentGroupIdentifier { get; set; }
        public Guid? GroupStatusIdentifier { get; set; }
        public string GroupType { get; set; }
        public string GroupName { get; set; }
        public string GroupNameLike { get; set; }
        public string GroupCode { get; set; }

        public string Address { get; set; }
        public string Country { get; set; }
        public string[] Provinces { get; set; }
        public string[] Cities { get; set; }

        public Guid? AnyParentGroupIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? MembershipUserIdentifier { get; set; }
        public Guid[] OrganizationIdentifiers { get; set; }
        public Guid[] Statuses { get; set; }
        public Guid? ExcludeContainerIdentifier { get; set; }

        public string GroupCategory { get; set; }
        public string GroupLabel { get; set; }
        public string GroupRegion { get; set; }
        public DateTime? UtcCreatedSince { get; set; }
        public DateTimeOffset? GroupExpiryBefore { get; set; }

        public string[] AddressTypes { get; set; }
        public string[] GroupCodes { get; set; }

        public string Keyword { get; set; }
        public bool? AllowSelfSubscription { get; set; }
        public bool? AddNewUsersAutomatically { get; set; }

        public string SurveyNecessity { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public DateTimeOffset? GroupExpirySince { get; set; }
        public DateTime? UtcCreatedBefore { get; set; }

        public Guid? ExcludeJournalSetupIdentifier { get; set; }

        public bool? HasLifetime { get; set; }

        public Guid? MembershipProductIdentifier { get; set; }

        public bool? OnlyOperatorCanAddUser { get; set; }
    }
}
