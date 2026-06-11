using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupSettings : Command
    {
        public bool AddNewUsersAutomatically { get; }
        public bool AllowSelfSubscription { get; }

        public ChangeGroupSettings(Guid group, bool addNewUsersAutomatically, bool allowSelfSubscription)
        {
            AggregateIdentifier = group;
            AddNewUsersAutomatically = addNewUsersAutomatically;
            AllowSelfSubscription = allowSelfSubscription;
        }
    }
}
