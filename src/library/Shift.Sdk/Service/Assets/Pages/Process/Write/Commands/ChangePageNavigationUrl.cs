using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageNavigationUrl : Command
    {
        public string NavigateUrl { get; set; }
        public ChangePageNavigationUrl(Guid page, string navigateUrl)
        {
            AggregateIdentifier = page;
            NavigateUrl = navigateUrl;
        }
    }
}
