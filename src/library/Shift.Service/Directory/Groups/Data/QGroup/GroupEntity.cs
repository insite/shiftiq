using Shift.Service.Security;

namespace Shift.Service.Directory;

public partial class GroupEntity
{
    public ICollection<MembershipEntity> Memberships { get; set; } = new List<MembershipEntity>();
    public ICollection<TPermissionEntity> Permissions { get; set; } = new List<TPermissionEntity>();

    public Guid GroupIdentifier { get; set; }
    public Guid? MembershipProductIdentifier { get; set; }
    public Guid? MessageToAdminWhenEventVenueChanged { get; set; }
    public Guid? MessageToAdminWhenMembershipEnded { get; set; }
    public Guid? MessageToAdminWhenMembershipStarted { get; set; }
    public Guid? MessageToUserWhenMembershipEnded { get; set; }
    public Guid? MessageToUserWhenMembershipStarted { get; set; }
    public Guid OrganizationIdentifier { get; set; }
    public Guid? ParentGroupIdentifier { get; set; }
    public Guid? SurveyFormIdentifier { get; set; }
    public Guid? GroupStatusItemIdentifier { get; set; }

    public bool AddNewUsersAutomatically { get; set; }
    public bool AllowJoinGroupUsingLink { get; set; }
    public bool AllowSelfSubscription { get; set; }

    public string? GroupCategory { get; set; }
    public string? GroupCode { get; set; }
    public string? GroupDescription { get; set; }
    public string? GroupEmail { get; set; }
    public string? GroupFax { get; set; }
    public string? GroupImage { get; set; }
    public string? GroupIndustry { get; set; }
    public string? GroupIndustryComment { get; set; }
    public string? GroupLabel { get; set; }
    public string GroupName { get; set; } = null!;
    public string? GroupOffice { get; set; }
    public string? GroupPhone { get; set; }
    public string? GroupRegion { get; set; }
    public string? GroupSize { get; set; }
    public string GroupType { get; set; } = null!;
    public string? GroupWebSiteUrl { get; set; }
    public string LastChangeType { get; set; } = null!;
    public string LastChangeUser { get; set; } = null!;
    public string? LifetimeUnit { get; set; }
    public string? ShippingPreference { get; set; }
    public string? SocialMediaUrls { get; set; }
    public string? SurveyNecessity { get; set; }

    public int? GroupCapacity { get; set; }
    public int? LifetimeQuantity { get; set; }

    public DateTimeOffset GroupCreated { get; set; }
    public DateTimeOffset? GroupExpiry { get; set; }
    public DateTimeOffset LastChangeTime { get; set; }
}