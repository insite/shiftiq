using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageTitle : Command
    {
        public string Title { get; set; }
        public ChangePageTitle(Guid page, string title)
        {
            AggregateIdentifier = page;
            Title = title;
        }
    }
}
