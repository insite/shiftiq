using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Pages.Write
{
    public class ChangePageContent : Command
    {
        public ContentContainer Content { get; }

        public ChangePageContent(Guid page, ContentContainer content)
        {
            AggregateIdentifier = page;
            Content = content;
        }
    }
}
