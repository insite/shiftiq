using System;
using System.Collections.Generic;
using System.ComponentModel;

using Shift.Common.Timeline.Changes;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Contacts
{
    [Serializable]
    public class GroupState : AggregateState
    {
        public Guid Identifier { get; set; }
        public Guid Tenant { get; set; }
        public Guid? Parent { get; set; }
        public Guid? Survey { get; set; }
        public Guid? MembershipProduct { get; set; }
        public Guid? StatusId { get; set; }

        public Guid? MessageToAdminWhenEventVenueChanged { get; set; }
        public Guid? MessageToAdminWhenMembershipEnded { get; set; }
        public Guid? MessageToAdminWhenMembershipStarted { get; set; }

        public Guid? MessageToUserWhenMembershipEnded { get; set; }
        public Guid? MessageToUserWhenMembershipStarted { get; set; }

        public string Type { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public int? Capacity { get; set; }
        public string Image { get; set; }
        public string Industry { get; set; }
        public string IndustryComment { get; set; }
        public string Office { get; set; }
        public string Email { get; set; }
        public string Region { get; set; }
        public string ShippingPreference { get; set; }
        public string WebSiteUrl { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public bool AddNewUsersAutomatically { get; set; }
        public bool AllowSelfSubscription { get; set; }
        public bool AllowJoinGroupUsingLink { get; set; }
        public bool OnlyOperatorCanAddUser { get; set; }
        public string Size { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> SocialMediaUrls { get; set; } = new Dictionary<string, string>();
        public List<string> Tags { get; set; } = new List<string>();

        public int? LifetimeQuantity { get; set; }
        public string LifetimeUnit { get; set; }
        public DateTimeOffset? Expiry { get; set; }

        [DefaultValue(Necessity.Required)]
        public Necessity SurveyNecessity { get; set; } = Necessity.Required;

        public Dictionary<AddressType, GroupAddress> Addresses { get; set; } = new Dictionary<AddressType, GroupAddress>();
        public Dictionary<Guid, ConnectionType> Connections { get; set; } = new Dictionary<Guid, ConnectionType>();

        public GroupAddress FindAddress(AddressType type)
            => Addresses.TryGetValue(type, out var address) ? address : null;

        public void When(GroupAddressChanged e)
        {
            if (e.Address != null)
                Addresses[e.Type] = e.Address;
            else
                Addresses.Remove(e.Type);
        }

        public void When(GroupCapacityChanged e)
        {
            Capacity = e.Capacity;
        }

        public void When(GroupConnected e)
        {
            Connections.Add(e.Group, e.ConnectionType);
        }

        public void When(GroupCreated e)
        {
            Tenant = e.Tenant;
            Type = e.Type;
            Name = e.Name;
        }

        public void When(GroupDeleted _)
        {
        }

        public void When(GroupDescribed e)
        {
            Category = e.Category;
            Code = e.Code;
            Description = e.Description;
            Label = e.Label;
        }

        public void When(GroupDisconnected e)
        {
            Connections.Remove(e.Group);
        }

        public void When(GroupImageChanged e)
        {
            Image = e.Image;
        }

        public void When(GroupIndustryChanged e)
        {
            Industry = e.Industry;
            IndustryComment = e.IndustryComment;
        }

        public void When(GroupLocationChanged e)
        {
            Office = e.Office;
            Region = e.Region;
            ShippingPreference = e.ShippingPreference;
            WebSiteUrl = e.WebSiteUrl;
        }

        public void When(GroupNotificationsConfigured e)
        {
            MessageToAdminWhenEventVenueChanged = e.MessageToAdminWhenEventVenueChanged;
            MessageToAdminWhenMembershipEnded = e.MessageToAdminWhenMembershipEnded;
            MessageToAdminWhenMembershipStarted = e.MessageToAdminWhenMembershipStarted;

            MessageToUserWhenMembershipEnded = e.MessageToUserWhenMembershipEnded;
            MessageToUserWhenMembershipStarted = e.MessageToUserWhenMembershipStarted;
        }

        public void When(GroupParentChanged e)
        {
            Parent = e.Parent;
        }

        public void When(GroupPhoneChanged e)
        {
            Phone = e.Phone;
            Fax = e.Fax;
        }

        public void When(GroupRenamed e)
        {
            Type = e.Type;
            Name = e.Name.MaxLength(90);
        }

        public void When(GroupSettingsChanged e)
        {
            AddNewUsersAutomatically = e.AddNewUsersAutomatically;
            AllowSelfSubscription = e.AllowSelfSubscription;
        }

        public void When(AllowJoinGroupUsingLinkModified e)
        {
            AllowJoinGroupUsingLink = e.AllowJoinGroupUsingLink;
        }

        public void When(GroupSizeChanged e)
        {
            Size = e.Size;
        }

        public void When(GroupStatusModified e)
        {
            StatusId = e.StatusId;
        }

        public void When(GroupEmailChanged e)
        {
            Email = e.Email;
        }

        public void When(GroupWebSiteUrlChanged e)
        {
            WebSiteUrl = e.WebSiteUrl;
        }

        public void When(GroupSocialMediaUrlChanged e)
        {
            if (SocialMediaUrls.ContainsKey(e.Type))
            {
                if (e.Url != null)
                    SocialMediaUrls[e.Type] = e.Url;
                else
                    SocialMediaUrls.Remove(e.Type);
            }
            else
            {
                SocialMediaUrls.Add(e.Type, e.Url);
            }
        }

        public void When(GroupTagAdded e)
        {
            if (!Tags.Contains(e.Tag))
                Tags.Add(e.Tag);
        }

        public void When(GroupTagRemoved e)
        {
            if (Tags.Contains(e.Tag))
                Tags.Remove(e.Tag);
        }

        public void When(GroupSurveyChanged e)
        {
            Survey = e.Survey;
            SurveyNecessity = e.Necessity;
        }

        public void When(GroupExpiryChanged e)
        {
            Expiry = e.Expiry;
        }

        public void When(GroupLifetimeChanged e)
        {
            LifetimeQuantity = e.Quantity;
            LifetimeUnit = e.Unit;
        }

        public void When(GroupExpired e)
        {
            Expiry = e.Expiry;
        }

        public void When(GroupMembershipProductModified e)
        {
            MembershipProduct = e.MembershipProduct;
        }

        public void When(GroupOnlyOperatorCanAddUserModified e)
        {
            OnlyOperatorCanAddUser = e.OnlyOperatorCanAddUser;
        }

        public void When(SerializedChange _)
        {
            // Obsolete changes go here
        }
    }
}