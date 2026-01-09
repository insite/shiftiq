using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageSite : Command
    {
        public Guid? Site { get; set; }
        public ChangePageSite(Guid page, Guid? site)
        {
            AggregateIdentifier = page;
            Site = site;
        }
    }
}
