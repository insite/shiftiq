using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageType : Command
    {
        public string Type { get; set; }
        public ChangePageType(Guid page, string type)
        {
            AggregateIdentifier = page;
            Type = type;
        }
    }
}
