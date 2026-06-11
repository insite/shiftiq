using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupEmail : Command
    {
        public string Email { get; }

        public ChangeGroupEmail(Guid group, string email)
        {
            AggregateIdentifier = group;
            Email = email.NullIfEmpty();
        }
    }
}
