using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class ModifyUserPassword : Command
    {
        public string PasswordHash { get; set; }
        public DateTimeOffset? PasswordChanged { get; set; }
        public DateTimeOffset PasswordExpired { get; set; }

        public ModifyUserPassword(Guid userId, string passwordHash, DateTimeOffset? passwordChanged, DateTimeOffset passwordExpired)
        {
            AggregateIdentifier = userId;
            PasswordHash = passwordHash;
            PasswordChanged = passwordChanged;
            PasswordExpired = passwordExpired;
        }
    }
}
