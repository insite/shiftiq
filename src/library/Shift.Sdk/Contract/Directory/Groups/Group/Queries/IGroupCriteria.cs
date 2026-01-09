using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGroupCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? MembershipProductIdentifier { get; set; }
        Guid? MessageToAdminWhenEventVenueChanged { get; set; }
        Guid? MessageToAdminWhenMembershipEnded { get; set; }
        Guid? MessageToAdminWhenMembershipStarted { get; set; }
        Guid? MessageToUserWhenMembershipEnded { get; set; }
        Guid? MessageToUserWhenMembershipStarted { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? ParentGroupIdentifier { get; set; }
        Guid? SurveyFormIdentifier { get; set; }

        bool? AddNewUsersAutomatically { get; set; }
        bool? AllowJoinGroupUsingLink { get; set; }
        bool? AllowSelfSubscription { get; set; }

        string GroupCategory { get; set; }
        string GroupCode { get; set; }
        string GroupDescription { get; set; }
        string GroupEmail { get; set; }
        string GroupFax { get; set; }
        string GroupImage { get; set; }
        string GroupIndustry { get; set; }
        string GroupIndustryComment { get; set; }
        string GroupLabel { get; set; }
        string GroupName { get; set; }
        string GroupOffice { get; set; }
        string GroupPhone { get; set; }
        string GroupRegion { get; set; }
        string GroupSize { get; set; }
        string GroupStatus { get; set; }
        string GroupType { get; set; }
        string GroupWebSiteUrl { get; set; }
        string LastChangeType { get; set; }
        string LastChangeUser { get; set; }
        string LifetimeUnit { get; set; }
        string ShippingPreference { get; set; }
        string SocialMediaUrls { get; set; }
        string SurveyNecessity { get; set; }

        int? GroupCapacity { get; set; }
        int? LifetimeQuantity { get; set; }

        DateTimeOffset? GroupCreated { get; set; }
        DateTimeOffset? GroupExpiry { get; set; }
        DateTimeOffset? LastChangeTime { get; set; }
    }
}