using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupImage : Command
    {
        public string Image { get; }

        public ChangeGroupImage(Guid group, string image)
        {
            AggregateIdentifier = group;
            Image = image.NullIfEmpty();
        }
    }
}
