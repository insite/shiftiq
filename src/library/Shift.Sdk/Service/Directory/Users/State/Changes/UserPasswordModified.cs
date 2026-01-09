using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class UserPasswordModified : Change
    {
        public string PasswordHash { get; set; }
        public DateTimeOffset? PasswordChanged { get; set; }
        public DateTimeOffset PasswordExpired { get; set; }

        public UserPasswordModified(string passwordHash, DateTimeOffset? passwordChanged, DateTimeOffset passwordExpired)
        {
            PasswordHash = passwordHash;
            PasswordChanged = passwordChanged;
            PasswordExpired = passwordExpired;
        }
    }
}
