using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class DeleteUser : Command
    {
        public DeleteUser(Guid userId)
        {
            AggregateIdentifier = userId;
        }
    }
}
