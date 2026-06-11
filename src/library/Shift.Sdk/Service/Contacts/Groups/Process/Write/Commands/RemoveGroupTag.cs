using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Groups.Write
{
    public class RemoveGroupTag : Command
    {
        public string Tag { get; }

        public RemoveGroupTag(Guid group, string tag)
        {
            AggregateIdentifier = group;
            Tag = tag.NullIfEmpty();
        }
    }
}
