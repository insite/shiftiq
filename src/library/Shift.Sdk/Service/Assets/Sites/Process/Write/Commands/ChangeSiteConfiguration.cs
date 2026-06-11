using System;

using Shift.Common.Timeline.Commands;


namespace InSite.Application.Sites.Write
{
    public class ChangeSiteConfiguration : Command
    {
        public string Configuration { get; set; }
        public ChangeSiteConfiguration(Guid site, string configuration)
        {
            AggregateIdentifier = site;
            Configuration = configuration;
        }
    }
}
