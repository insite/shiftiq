using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageNewTabValue : Command
    {
        public bool IsNewTab { get; set; }
        public ChangePageNewTabValue(Guid page, bool isNewTab)
        {
            AggregateIdentifier = page;
            IsNewTab = isNewTab;
        }
    }
}
