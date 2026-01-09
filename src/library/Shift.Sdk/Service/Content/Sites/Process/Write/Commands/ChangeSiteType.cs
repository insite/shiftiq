using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Sites.Write
{
    public class ChangeSiteType : Command
    {
        public string Type { get; set; }

        public ChangeSiteType(Guid site, string type)
        {
            AggregateIdentifier = site;
            Type = type;
        }
    }
}