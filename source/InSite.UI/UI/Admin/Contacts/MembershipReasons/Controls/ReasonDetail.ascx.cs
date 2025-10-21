using System;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.MembershipReasons.Controls
{
    public partial class ReasonDetail : BaseUserControl
    {
        private (Guid Membership, Guid Group)[] Memberships
        {
            get => ((Guid, Guid)[])ViewState[nameof(Memberships)];
            set => ViewState[nameof(Memberships)] = value;
        }

        private void LoadMemberships(Guid userId)
        {
            Memberships = MembershipSearch
                .Bind(
                    x => new
                    {
                        x.MembershipIdentifier,
                        x.GroupIdentifier
                    },
                    x => x.UserIdentifier == userId && x.Group.OrganizationIdentifier == Organization.OrganizationIdentifier)
                .Select(x => (x.MembershipIdentifier, x.GroupIdentifier))
                .ToArray();

            var groupIds = Memberships.Select(x => x.Group).ToArray();
            if (groupIds.IsEmpty())
                groupIds = new[] { Guid.Empty };

            MembershipGroup.Filter.IncludeGroupIdentifiers = groupIds;
            MembershipGroup.Value = null;
        }

        public void SetDefaultInputValues(Guid userId)
        {
            LoadMemberships(userId);

            MembershipGroup.Value = null;
            MembershipGroup.Enabled = true;

            ReasonSubtype.Value = null;
            PersonOccupation.Text = null;
            ReasonEffectiveDate.Value = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, User.TimeZone);
            ReasonExpiryDate.Value = null;
        }

        public void SetInputValues(QMembershipReason reason)
        {
            LoadMemberships(reason.Membership.UserIdentifier);
            MembershipGroup.Value = GetMembershipGroupId(reason.MembershipIdentifier);
            MembershipGroup.Enabled = false;

            ReasonSubtype.Value = reason.ReasonSubtype;
            PersonOccupation.Text = reason.PersonOccupation;
            ReasonEffectiveDate.Value = reason.ReasonEffective;
            ReasonExpiryDate.Value = reason.ReasonExpiry;
        }

        public void GetInputValues(QMembershipReason reason)
        {
            reason.MembershipIdentifier = GetGroupMembershipId(MembershipGroup.Value.Value).Value;
            reason.ReasonSubtype = ReasonSubtype.Value;
            reason.PersonOccupation = PersonOccupation.Text;
            reason.ReasonEffective = ReasonEffectiveDate.Value.Value;
            reason.ReasonExpiry = ReasonExpiryDate.Value;
        }

        private Guid? GetMembershipGroupId(Guid membershipId)
        {
            var mapping = Memberships.FirstOrDefault(x => x.Membership == membershipId);
            return mapping == default ? (Guid?)null : mapping.Group;
        }

        private Guid? GetGroupMembershipId(Guid groupId)
        {
            var mapping = Memberships.FirstOrDefault(x => x.Group == groupId);
            return mapping == default ? (Guid?)null : mapping.Membership;
        }
    }
}