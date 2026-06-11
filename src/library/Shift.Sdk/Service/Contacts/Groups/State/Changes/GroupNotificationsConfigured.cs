using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupNotificationsConfigured : Change
    {
        public Guid? MessageToAdminWhenEventVenueChanged { get; }
        public Guid? MessageToAdminWhenMembershipEnded { get; }
        public Guid? MessageToAdminWhenMembershipStarted { get; }

        public Guid? MessageToUserWhenMembershipEnded { get; }
        public Guid? MessageToUserWhenMembershipStarted { get; }

        public GroupNotificationsConfigured(
            Guid? messageToAdminOnEventVenueChanged,
            Guid? messageToAdminWhenMembershipEnded,
            Guid? messageToAdminOnGroupMembershipStarted,

            Guid? messageToUserWhenMembershipEnded,
            Guid? messageToUserOnGroupMembershipStarted
            )
        {
            MessageToAdminWhenEventVenueChanged = messageToAdminOnEventVenueChanged;
            MessageToAdminWhenMembershipEnded = messageToAdminWhenMembershipEnded;
            MessageToAdminWhenMembershipStarted = messageToAdminOnGroupMembershipStarted;

            MessageToUserWhenMembershipEnded = messageToUserWhenMembershipEnded;
            MessageToUserWhenMembershipStarted = messageToUserOnGroupMembershipStarted;
        }
    }
}
