using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class MembershipStarted : Change
    {
        public MembershipStarted(Guid user, Guid group, string function, DateTimeOffset effective)
        {
            User = user;
            Group = group;
            Function = function;
            Effective = effective;
        }

        public Guid User { get; }
        public Guid Group { get; }
        public string Function { get; }
        public DateTimeOffset Effective { get; }
    }
}