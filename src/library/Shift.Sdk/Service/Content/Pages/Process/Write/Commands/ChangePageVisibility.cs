using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageVisibility : Command
    {
        public bool IsHidden { get; set; }
        public ChangePageVisibility(Guid page, bool isHidden)
        {
            AggregateIdentifier = page;
            IsHidden = isHidden;
        }
    }
}
