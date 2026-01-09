using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageParent : Command
    {
        public Guid? Parent { get; set; }
        public ChangePageParent(Guid page, Guid? parent)
        {
            AggregateIdentifier = page;
            Parent = parent;
        }
    }
}
