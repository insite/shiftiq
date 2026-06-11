using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Sites
{
    public class SiteCreated : Change
    {
        public Guid Tenant { get; set; }
        public Guid Author { get; set; }

        public string Domain { get; set; }
        public string Title { get; set; }

        public SiteCreated(Guid tenant, Guid author, string type, string domain, string title, string configuration)
        {
            Tenant = tenant;
            Author = author;

            Domain = domain;
            Title = title;
        }
    }
}