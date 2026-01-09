using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageHook : Command
    {
        public string Hook { get; set; }
        public ChangePageHook(Guid page, string hook)
        {
            AggregateIdentifier = page;
            Hook = hook;
        }
    }
}
