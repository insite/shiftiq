using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupSize : Command
    {
        public string Size { get; }

        public ChangeGroupSize(Guid group, string size)
        {
            AggregateIdentifier = group;
            Size = size.NullIfEmpty();
        }
    }
}
