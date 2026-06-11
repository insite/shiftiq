using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageContentLabels : Command
    {
        public string ContentLabels { get; set; }
        public ChangePageContentLabels(Guid page, string contentLabels)
        {
            AggregateIdentifier = page;
            ContentLabels = contentLabels;
        }
    }
}
