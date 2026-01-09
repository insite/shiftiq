using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class ModifyUserDefaultPassword : Command
    {
        public string DefaultPassword { get; set; }
        public DateTimeOffset? DefaultPasswordExpired { get; set; }

        public ModifyUserDefaultPassword(Guid userId, string defaultPassword, DateTimeOffset? defaultPasswordExpired)
        {
            AggregateIdentifier = userId;
            DefaultPassword = defaultPassword;
            DefaultPasswordExpired = defaultPasswordExpired;
        }
    }
}
