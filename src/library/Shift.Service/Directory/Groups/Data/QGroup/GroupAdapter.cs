using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class GroupAdapter : IEntityAdapter
{
    public void Copy(ModifyGroup modify, GroupEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.ParentGroupIdentifier = modify.ParentGroupIdentifier;
        entity.SurveyFormIdentifier = modify.SurveyFormIdentifier;
        entity.GroupStatusItemIdentifier = modify.GroupStatusItemIdentifier;
        entity.MessageToUserWhenMembershipStarted = modify.MessageToUserWhenMembershipStarted;
        entity.MessageToAdminWhenMembershipStarted = modify.MessageToAdminWhenMembershipStarted;
        entity.MessageToAdminWhenEventVenueChanged = modify.MessageToAdminWhenEventVenueChanged;
        entity.AddNewUsersAutomatically = modify.AddNewUsersAutomatically;
        entity.AllowSelfSubscription = modify.AllowSelfSubscription;
        entity.GroupCapacity = modify.GroupCapacity;
        entity.GroupCategory = modify.GroupCategory;
        entity.GroupCode = modify.GroupCode;
        entity.GroupCreated = modify.GroupCreated;
        entity.GroupType = modify.GroupType;
        entity.GroupDescription = modify.GroupDescription;
        entity.GroupFax = modify.GroupFax;
        entity.GroupImage = modify.GroupImage;
        entity.GroupIndustry = modify.GroupIndustry;
        entity.GroupIndustryComment = modify.GroupIndustryComment;
        entity.GroupLabel = modify.GroupLabel;
        entity.GroupName = modify.GroupName;
        entity.GroupOffice = modify.GroupOffice;
        entity.GroupPhone = modify.GroupPhone;
        entity.GroupRegion = modify.GroupRegion;
        entity.GroupSize = modify.GroupSize;
        entity.GroupWebSiteUrl = modify.GroupWebSiteUrl;
        entity.ShippingPreference = modify.ShippingPreference;
        entity.SurveyNecessity = modify.SurveyNecessity;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
        entity.GroupEmail = modify.GroupEmail;
        entity.SocialMediaUrls = modify.SocialMediaUrls;
        entity.GroupExpiry = modify.GroupExpiry;
        entity.LifetimeUnit = modify.LifetimeUnit;
        entity.LifetimeQuantity = modify.LifetimeQuantity;
        entity.MessageToAdminWhenMembershipEnded = modify.MessageToAdminWhenMembershipEnded;
        entity.MessageToUserWhenMembershipEnded = modify.MessageToUserWhenMembershipEnded;
        entity.MembershipProductIdentifier = modify.MembershipProductIdentifier;
        entity.AllowJoinGroupUsingLink = modify.AllowJoinGroupUsingLink;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public GroupEntity ToEntity(CreateGroup create)
    {
        var entity = new GroupEntity
        {
            GroupIdentifier = create.GroupIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            ParentGroupIdentifier = create.ParentGroupIdentifier,
            SurveyFormIdentifier = create.SurveyFormIdentifier,
            MessageToUserWhenMembershipStarted = create.MessageToUserWhenMembershipStarted,
            MessageToAdminWhenMembershipStarted = create.MessageToAdminWhenMembershipStarted,
            MessageToAdminWhenEventVenueChanged = create.MessageToAdminWhenEventVenueChanged,
            AddNewUsersAutomatically = create.AddNewUsersAutomatically,
            AllowSelfSubscription = create.AllowSelfSubscription,
            GroupCapacity = create.GroupCapacity,
            GroupCategory = create.GroupCategory,
            GroupCode = create.GroupCode,
            GroupCreated = create.GroupCreated,
            GroupType = create.GroupType,
            GroupDescription = create.GroupDescription,
            GroupFax = create.GroupFax,
            GroupImage = create.GroupImage,
            GroupIndustry = create.GroupIndustry,
            GroupIndustryComment = create.GroupIndustryComment,
            GroupLabel = create.GroupLabel,
            GroupName = create.GroupName,
            GroupOffice = create.GroupOffice,
            GroupPhone = create.GroupPhone,
            GroupRegion = create.GroupRegion,
            GroupSize = create.GroupSize,
            GroupStatusItemIdentifier = create.GroupStatusItemIdentifier,
            GroupWebSiteUrl = create.GroupWebSiteUrl,
            ShippingPreference = create.ShippingPreference,
            SurveyNecessity = create.SurveyNecessity,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser,
            GroupEmail = create.GroupEmail,
            SocialMediaUrls = create.SocialMediaUrls,
            GroupExpiry = create.GroupExpiry,
            LifetimeUnit = create.LifetimeUnit,
            LifetimeQuantity = create.LifetimeQuantity,
            MessageToAdminWhenMembershipEnded = create.MessageToAdminWhenMembershipEnded,
            MessageToUserWhenMembershipEnded = create.MessageToUserWhenMembershipEnded,
            MembershipProductIdentifier = create.MembershipProductIdentifier,
            AllowJoinGroupUsingLink = create.AllowJoinGroupUsingLink
        };
        return entity;
    }

    public IEnumerable<GroupModel> ToModel(IEnumerable<GroupEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public GroupModel ToModel(GroupEntity entity)
    {
        var model = new GroupModel
        {
            GroupIdentifier = entity.GroupIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            ParentGroupIdentifier = entity.ParentGroupIdentifier,
            SurveyFormIdentifier = entity.SurveyFormIdentifier,
            MessageToUserWhenMembershipStarted = entity.MessageToUserWhenMembershipStarted,
            MessageToAdminWhenMembershipStarted = entity.MessageToAdminWhenMembershipStarted,
            MessageToAdminWhenEventVenueChanged = entity.MessageToAdminWhenEventVenueChanged,
            AddNewUsersAutomatically = entity.AddNewUsersAutomatically,
            AllowSelfSubscription = entity.AllowSelfSubscription,
            GroupCapacity = entity.GroupCapacity,
            GroupCategory = entity.GroupCategory,
            GroupCode = entity.GroupCode,
            GroupCreated = entity.GroupCreated,
            GroupType = entity.GroupType,
            GroupDescription = entity.GroupDescription,
            GroupFax = entity.GroupFax,
            GroupImage = entity.GroupImage,
            GroupIndustry = entity.GroupIndustry,
            GroupIndustryComment = entity.GroupIndustryComment,
            GroupLabel = entity.GroupLabel,
            GroupName = entity.GroupName,
            GroupOffice = entity.GroupOffice,
            GroupPhone = entity.GroupPhone,
            GroupRegion = entity.GroupRegion,
            GroupSize = entity.GroupSize,
            GroupStatusItemIdentifier = entity.GroupStatusItemIdentifier,
            GroupWebSiteUrl = entity.GroupWebSiteUrl,
            ShippingPreference = entity.ShippingPreference,
            SurveyNecessity = entity.SurveyNecessity,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser,
            GroupEmail = entity.GroupEmail,
            SocialMediaUrls = entity.SocialMediaUrls,
            GroupExpiry = entity.GroupExpiry,
            LifetimeUnit = entity.LifetimeUnit,
            LifetimeQuantity = entity.LifetimeQuantity,
            MessageToAdminWhenMembershipEnded = entity.MessageToAdminWhenMembershipEnded,
            MessageToUserWhenMembershipEnded = entity.MessageToUserWhenMembershipEnded,
            MembershipProductIdentifier = entity.MembershipProductIdentifier,
            AllowJoinGroupUsingLink = entity.AllowJoinGroupUsingLink
        };

        return model;
    }

    public IEnumerable<GroupMatch> ToMatch(IEnumerable<GroupEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public GroupMatch ToMatch(GroupEntity entity)
    {
        var match = new GroupMatch
        {
            GroupIdentifier = entity.GroupIdentifier,
            GroupName = entity.GroupName
        };

        return match;
    }
}