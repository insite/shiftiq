using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Sites.Write
{
    public class ChangeSiteContent : Command
    {
        public ContentContainer Content { get; }

        public ChangeSiteContent(Guid site, ContentContainer content)
        {
            AggregateIdentifier = site;
            Content = content;
        }
    }
}
