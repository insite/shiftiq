using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Sites.Write
{
    public class CreateSite : Command
    {
        public Guid Tenant { get; set; }
        public Guid Author { get; set; }

        public string Domain { get; set; }
        public string Title { get; set; }

        public CreateSite(Guid site, Guid tenant, Guid author, string type, string domain, string title, string configuration)
        {
            AggregateIdentifier = site;

            Tenant = tenant;
            Author = author;

            Domain = domain;
            Title = title;
        }
    }
}