using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Groups.Write
{
    public class AddGroupTag : Command
    {
        public string Tag { get; }

        public AddGroupTag(Guid group, string tag)
        {
            AggregateIdentifier = group;
            Tag = tag.NullIfEmpty();
        }
    }
}
