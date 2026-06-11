using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Contacts
{
    public class GroupAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new GroupState();

        public GroupState Data => (GroupState)State;

        public void ConnectGroup(Guid container, ConnectionType connectionType)
        {
            Apply(new GroupConnected(container, connectionType));
        }

        public void ChangeGroupAddress(AddressType type, GroupAddress address)
        {
            Apply(new GroupAddressChanged(type, address));
        }

        public void ChangeGroupCapacity(int? capacity)
        {
            Apply(new GroupCapacityChanged(capacity));
        }

        public void ChangeGroupImage(string image)
        {
            Apply(new GroupImageChanged(image));
        }

        public void ChangeGroupIndustry(string industry, string industryComment)
        {
            Apply(new GroupIndustryChanged(industry, industryComment));
        }

        public void ChangeGroupLocation(string office, string region, string shippingPreference, string webSiteUrl)
        {
            Apply(new GroupLocationChanged(office, region, shippingPreference, webSiteUrl));
        }

        public void ChangeGroupParent(Guid? parent)
        {
            Apply(new GroupParentChanged(parent));
        }

        public void ChangeGroupPhone(string phone)
        {
            Apply(new GroupPhoneChanged(phone, null));
        }

        public void ChangeGroupSettings(bool addNewUsersAutomatically, bool allowSelfSubscription)
        {
            Apply(new GroupSettingsChanged(addNewUsersAutomatically, allowSelfSubscription));
        }

        public void ModifyAllowJoinGroupUsingLink(bool allowJoinGroupUsingLink)
        {
            Apply(new AllowJoinGroupUsingLinkModified(allowJoinGroupUsingLink));
        }

        public void ChangeGroupSize(string size)
        {
            Apply(new GroupSizeChanged(size));
        }

        public void AddGroupTag(string tag)
        {
            Apply(new GroupTagAdded(tag));
        }

        public void RemoveGroupTag(string tag)
        {
            Apply(new GroupTagRemoved(tag));
        }

        public void ChangeGroupEmail(string email)
        {
            Apply(new GroupEmailChanged(email));
        }

        public void ChangeGroupURL(string url)
        {
            Apply(new GroupWebSiteUrlChanged(url));
        }

        public void ChangeSocialMediaUrl(string type, string url)
        {
            bool isChanged;

            // Proceed with the change only if one (or both) of the following conditions are true:
            // 1. The social media link already exists in the list and its URL has been modified.
            // 2. The social media link does not exist and the URL is not empty.

            if (Data.SocialMediaUrls.ContainsKey(type))
            {
                var existing = Data.SocialMediaUrls[type];
                isChanged = !StringHelper.Equals(existing, url);
            }
            else
            {
                isChanged = !string.IsNullOrWhiteSpace(url);
            }

            if (isChanged)
                Apply(new GroupSocialMediaUrlChanged(type, url));
        }

        public void ChangeGroupSurvey(Guid? survey, Necessity necessity)
        {
            Apply(new GroupSurveyChanged(survey, necessity));
        }

        public void ConfigureGroupNotifications(
            Guid? messageToAdminWhenEventVenueChanged,
            Guid? messageToAdminWhenMembershipEnded,
            Guid? messageToAdminWhenMembershipStarted,
            Guid? messageToUserWhenMembershipEnded,
            Guid? messageToUserWhenMembershipStarted
            )
        {
            Apply(new GroupNotificationsConfigured(
                messageToAdminWhenEventVenueChanged,
                messageToAdminWhenMembershipEnded,
                messageToAdminWhenMembershipStarted,
                messageToUserWhenMembershipEnded,
                messageToUserWhenMembershipStarted
                ));
        }

        public void CreateGroup(Guid organization, string type, string name)
        {
            Apply(new GroupCreated(organization, type, name, DateTimeOffset.UtcNow));
        }

        public void DeleteGroup(string reason)
        {
            Apply(new GroupDeleted(reason));
        }

        public void DescribeGroup(string category, string code, string description, string label)
        {
            Apply(new GroupDescribed(category, code, description, label));
        }

        public void DisconnectGroup(Guid group)
        {
            Apply(new GroupDisconnected(group));
        }

        public void RenameGroup(string type, string name)
        {
            Apply(new GroupRenamed(type, name));
        }

        public void ChangeGroupExpiry(DateTimeOffset? expiry)
        {
            Apply(new GroupExpiryChanged(expiry));
        }

        public void ChangeGroupLifetime(int? quantity, string unit)
        {
            Apply(new GroupLifetimeChanged(quantity, unit));
        }

        public void ExpireGroup(DateTimeOffset expiry)
        {
            Apply(new GroupExpired(expiry));
        }

        public void ModifyGroupMembershipProduct(Guid? membershipProduct)
        {
            Apply(new GroupMembershipProductModified(membershipProduct));
        }

        public void ModifyGroupStatus(Guid? statusId)
        {
            Apply(new GroupStatusModified(statusId));
        }

        public void ModifyGroupOnlyOperatorCanAddUser(bool onlyOperatorCanAddUser)
        {
            Apply(new GroupOnlyOperatorCanAddUserModified(onlyOperatorCanAddUser));
        }
    }
}
