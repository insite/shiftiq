using Shift.Common.Timeline.Changes;

using InSite.Domain.Contacts;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    /// <summary>
    /// Implements the projector for Journal changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Changes can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from modifications to the projection tables). A processor,
    /// in contrast, should *never* replay past changes.
    /// </remarks>
    public class GroupChangeProjector
    {
        private readonly IGroupStore _store;

        public GroupChangeProjector(IChangeQueue publisher, IChangeStore changeStore, IGroupStore store)
        {
            _store = store;

            publisher.Subscribe<GroupAddressChanged>(Handle);
            publisher.Subscribe<GroupCapacityChanged>(Handle);
            publisher.Subscribe<GroupConnected>(Handle);
            publisher.Subscribe<GroupCreated>(Handle);
            publisher.Subscribe<GroupDeleted>(Handle);
            publisher.Subscribe<GroupDescribed>(Handle);
            publisher.Subscribe<GroupDisconnected>(Handle);
            publisher.Subscribe<GroupImageChanged>(Handle);
            publisher.Subscribe<GroupIndustryChanged>(Handle);
            publisher.Subscribe<GroupLocationChanged>(Handle);
            publisher.Subscribe<GroupNotificationsConfigured>(Handle);
            publisher.Subscribe<GroupParentChanged>(Handle);
            publisher.Subscribe<GroupPhoneChanged>(Handle);
            publisher.Subscribe<GroupRenamed>(Handle);
            publisher.Subscribe<GroupSettingsChanged>(Handle);
            publisher.Subscribe<AllowJoinGroupUsingLinkModified>(Handle);
            publisher.Subscribe<GroupSizeChanged>(Handle);
            publisher.Subscribe<GroupStatusModified>(Handle);
            publisher.Subscribe<GroupSurveyChanged>(Handle);
            publisher.Subscribe<GroupEmailChanged>(Handle);
            publisher.Subscribe<GroupWebSiteUrlChanged>(Handle);
            publisher.Subscribe<GroupSocialMediaUrlChanged>(Handle);
            publisher.Subscribe<GroupTagAdded>(Handle);
            publisher.Subscribe<GroupTagRemoved>(Handle);
            publisher.Subscribe<GroupExpired>(Handle);
            publisher.Subscribe<GroupExpiryChanged>(Handle);
            publisher.Subscribe<GroupLifetimeChanged>(Handle);
            publisher.Subscribe<GroupMembershipProductModified>(Handle);
            publisher.Subscribe<GroupOnlyOperatorCanAddUserModified>(Handle);

            changeStore.RegisterObsoleteChangeTypes(new[]
            {
                ObsoleteChangeType.GroupStatusChanged
            });
        }

        public void Handle(GroupAddressChanged e)
            => _store.UpdateGroupAddress(e);

        public void Handle(GroupCapacityChanged e)
            => _store.UpdateGroup(e, x => x.GroupCapacity = e.Capacity);

        public void Handle(GroupConnected e)
            => _store.InsertGroupContainer(e);

        public void Handle(GroupCreated e)
            => _store.InsertGroup(e);

        public void Handle(GroupTagAdded e)
            => _store.InsertGroupTag(e);

        public void Handle(GroupDeleted e)
            => _store.DeleteGroup(e);

        public void Handle(GroupTagRemoved e)
            => _store.DeleteGroupTag(e);

        public void Handle(GroupDescribed e)
            => _store.UpdateGroup(e, x =>
            {
                x.GroupCategory = e.Category;
                x.GroupCode = e.Code;
                x.GroupDescription = e.Description;
                x.GroupLabel = e.Label;
            });

        public void Handle(GroupDisconnected e)
            => _store.DeleteGroupContainer(e);

        public void Handle(GroupImageChanged e)
            => _store.UpdateGroup(e, x => x.GroupImage = e.Image);

        public void Handle(GroupIndustryChanged e)
            => _store.UpdateGroup(e, x =>
            {
                x.GroupIndustry = e.Industry;
                x.GroupIndustryComment = e.IndustryComment;
            });

        public void Handle(GroupLocationChanged e)
            => _store.UpdateGroup(e, x =>
            {
                x.GroupOffice = e.Office;
                x.GroupRegion = e.Region;
                x.ShippingPreference = e.ShippingPreference;
                x.GroupWebSiteUrl = e.WebSiteUrl;
            });

        public void Handle(GroupNotificationsConfigured e)
            => _store.UpdateGroup(e, x =>
            {
                x.MessageToAdminWhenEventVenueChanged = e.MessageToAdminWhenEventVenueChanged;
                x.MessageToAdminWhenMembershipEnded = e.MessageToAdminWhenMembershipEnded;
                x.MessageToAdminWhenMembershipStarted = e.MessageToAdminWhenMembershipStarted;

                x.MessageToUserWhenMembershipEnded = e.MessageToUserWhenMembershipEnded;
                x.MessageToUserWhenMembershipStarted = e.MessageToUserWhenMembershipStarted;
            });

        public void Handle(GroupParentChanged e)
            => _store.UpdateGroup(e, x => x.ParentGroupIdentifier = e.Parent);

        public void Handle(GroupPhoneChanged e)
            => _store.UpdateGroup(e, x =>
            {
                x.GroupPhone = e.Phone;
            });

        public void Handle(GroupRenamed e)
            => _store.UpdateGroup(e, x =>
            {
                x.GroupType = e.Type;
                x.GroupName = e.Name.MaxLength(90);
            });

        public void Handle(GroupSettingsChanged e)
            => _store.UpdateGroup(e, x =>
            {
                x.AddNewUsersAutomatically = e.AddNewUsersAutomatically;
                x.AllowSelfSubscription = e.AllowSelfSubscription;
            });

        public void Handle(AllowJoinGroupUsingLinkModified e)
            => _store.UpdateGroup(e, x =>
            {
                x.AllowJoinGroupUsingLink = e.AllowJoinGroupUsingLink;
            });

        public void Handle(GroupSizeChanged e)
            => _store.UpdateGroup(e, x => x.GroupSize = e.Size);

        public void Handle(GroupEmailChanged e)
            => _store.UpdateGroup(e, x => x.GroupEmail = e.Email);

        public void Handle(GroupWebSiteUrlChanged e)
            => _store.UpdateGroup(e, x => x.GroupWebSiteUrl = e.WebSiteUrl);

        public void Handle(GroupSocialMediaUrlChanged e)
            => _store.UpdateGroup(e);

        public void Handle(GroupStatusModified e)
            => _store.UpdateGroup(e, x => x.GroupStatusItemIdentifier = e.StatusId);

        public void Handle(GroupSurveyChanged e)
            => _store.UpdateGroup(e, x =>
            {
                x.SurveyFormIdentifier = e.Survey;
                x.SurveyNecessity = e.Necessity.ToString();
            });

        public void Handle(GroupExpired e)
            => _store.UpdateGroup(e, x =>
            {
                x.GroupExpiry = e.Expiry;
            });

        public void Handle(GroupExpiryChanged e)
            => _store.UpdateGroup(e, x =>
            {
                x.GroupExpiry = e.Expiry;
            });

        public void Handle(GroupLifetimeChanged e)
            => _store.UpdateGroup(e, x =>
            {
                x.LifetimeQuantity = e.Quantity;
                x.LifetimeUnit = e.Unit;
            });

        public void Handle(GroupMembershipProductModified e)
            => _store.UpdateGroup(e, x =>
            {
                x.MembershipProductIdentifier = e.MembershipProduct;
            });

        public void Handle(GroupOnlyOperatorCanAddUserModified e)
            => _store.UpdateGroup(e, x =>
            {
                x.OnlyOperatorCanAddUser = e.OnlyOperatorCanAddUser;
            });

        public void Handle(SerializedChange e)
        {
            // Obsolete changes go here

            if (e.ChangeType == ObsoleteChangeType.GroupStatusChanged)
                return;
        }
    }
}
