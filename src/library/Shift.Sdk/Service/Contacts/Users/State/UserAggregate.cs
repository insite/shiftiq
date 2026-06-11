using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class UserAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new UserState();

        public UserState Data => (UserState)State;

        public void CreateUser(string email, string firstName, string lastName, string middleName, string fullName, string timeZone)
        {
            Apply(new UserCreated(email, firstName, lastName, middleName, fullName, timeZone));
        }

        public void DeleteUser()
        {
            Apply(new UserDeleted());
        }

        public void ModifyUserDefaultPassword(string defaultPassword, DateTimeOffset? defaultPasswordExpired)
        {
            Apply(new UserDefaultPasswordModified(defaultPassword, defaultPasswordExpired));
        }

        public void ModifyUserName(string firstName, string lastName, string middleName, string fullName)
        {
            Apply(new UserNameModified(firstName, lastName, middleName, fullName));
        }

        public void ModifyUserPassword(string passwordHash, DateTimeOffset? passwordChanged, DateTimeOffset passwordExpired)
        {
            Apply(new UserPasswordModified(passwordHash, passwordChanged, passwordExpired));
        }

        public void ArchiveUser(DateTimeOffset date)
        {
            Apply(new UserArchived(date));
        }

        public void UnarchiveUser(DateTimeOffset date)
        {
            Apply(new UserUnarchived(date));
        }

        public void ConnectUser(Guid toUserId, bool isLeader, bool isManager, bool isSupervisor, bool isValidator, DateTimeOffset connected)
        {
            Apply(new UserConnected(toUserId, isLeader, isManager, isSupervisor, isValidator, connected));
        }

        public void DisconnectUser(Guid toUserId)
        {
            Apply(new UserDisconnected(toUserId));
        }

        public void ModifyUserTextField(UserField userField, string value)
        {
            Apply(new UserFieldTextModified(userField, value));
        }

        public void ModifyUserDateField(UserField userField, DateTimeOffset? value)
        {
            Apply(new UserFieldDateOffsetModified(userField, value));
        }

        public void ModifyUserBoolField(UserField userField, bool? value)
        {
            Apply(new UserFieldBoolModified(userField, value));
        }

        public void ModifyUserIntField(UserField userField, int? value)
        {
            Apply(new UserFieldIntModified(userField, value));
        }
    }
}
