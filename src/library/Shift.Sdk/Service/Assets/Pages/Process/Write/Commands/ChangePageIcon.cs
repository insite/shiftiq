using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageIcon : Command
    {
        public string Icon { get; set; }
        public ChangePageIcon(Guid page, string icon)
        {
            AggregateIdentifier = page;
            Icon = icon;
        }
    }
}
