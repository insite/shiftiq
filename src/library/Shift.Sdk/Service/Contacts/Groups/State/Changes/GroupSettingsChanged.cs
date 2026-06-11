using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupSettingsChanged : Change
    {
        public bool AddNewUsersAutomatically { get; }
        public bool AllowSelfSubscription { get; }

        public GroupSettingsChanged(bool addNewUsersAutomatically, bool allowSelfSubscription)
        {
            AddNewUsersAutomatically = addNewUsersAutomatically;
            AllowSelfSubscription = allowSelfSubscription;
        }
    }
}
