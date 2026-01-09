using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Sites.Write
{
    public class ChangeSiteDomain : Command
    {
        public string Domain { get; set; }

        public ChangeSiteDomain(Guid site, string domain)
        {
            AggregateIdentifier = site;
            Domain = domain;
        }
    }
}
