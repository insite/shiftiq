using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class UserDefaultPasswordModified : Change
    {
        public string DefaultPassword { get; set; }
        public DateTimeOffset? DefaultPasswordExpired { get; set; }

        public UserDefaultPasswordModified(string defaultPassword, DateTimeOffset? defaultPasswordExpired)
        {
            DefaultPassword = defaultPassword;
            DefaultPasswordExpired = defaultPasswordExpired;
        }
    }
}
