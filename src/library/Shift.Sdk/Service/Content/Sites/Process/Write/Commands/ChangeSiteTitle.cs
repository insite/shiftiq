using System;

using Shift.Common.Timeline.Commands;


namespace InSite.Application.Sites.Write
{
    public class ChangeSiteTitle : Command
    {
        public string Title { get; set; }
        public ChangeSiteTitle(Guid site, string title)
        {
            AggregateIdentifier = site;
            Title = title;
        }
    }
}
