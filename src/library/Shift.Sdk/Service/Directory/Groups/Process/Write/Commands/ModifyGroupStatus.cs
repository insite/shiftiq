using System;
using System.Collections.Generic;
using System.Text;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ModifyGroupStatus : Command
    {
        public Guid? StatusId { get; }

        public ModifyGroupStatus(Guid groupId, Guid? statusId)
        {
            AggregateIdentifier = groupId;
            StatusId = statusId;
        }
    }
}
