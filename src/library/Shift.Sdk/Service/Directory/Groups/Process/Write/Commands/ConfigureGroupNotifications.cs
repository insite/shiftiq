using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ConfigureGroupNotifications : Command
    {
        public Guid? MessageToAdminWhenEventVenueChanged { get; }
        public Guid? MessageToAdminWhenMembershipEnded { get; }
        public Guid? MessageToAdminWhenMembershipStarted { get; }

        public Guid? MessageToUserWhenMembershipEnded { get; }
        public Guid? MessageToUserWhenMembershipStarted { get; }

        public ConfigureGroupNotifications(
            Guid group,

            Guid? messageToAdminWhenEventVenueChanged,
            Guid? messageToAdminWhenMembershipEnded,
            Guid? messageToAdminWhenMembershipStarted,

            Guid? messageToUserWhenMembershipEnded,
            Guid? messageToUserWhenMembershipStarted
            )
        {
            AggregateIdentifier = group;

            MessageToAdminWhenEventVenueChanged = messageToAdminWhenEventVenueChanged;
            MessageToAdminWhenMembershipEnded = messageToAdminWhenMembershipEnded;
            MessageToAdminWhenMembershipStarted = messageToAdminWhenMembershipStarted;

            MessageToUserWhenMembershipEnded = messageToUserWhenMembershipEnded;
            MessageToUserWhenMembershipStarted = messageToUserWhenMembershipStarted;
        }
    }
}
