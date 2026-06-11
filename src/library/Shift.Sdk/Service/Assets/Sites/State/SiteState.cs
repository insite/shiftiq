using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Sites
{
    public class SiteState : AggregateState
    {
        public Guid Identifier { get; set; }

        public Guid Tenant { get; set; }
        public Guid Author { get; set; }

        public string Domain { get; set; }
        public string Title { get; set; }

        public Shift.Common.ContentContainer Content { get; set; }

        public SiteState()
        {

        }

        public void When(SiteConfigurationChanged c)
        {
            
        }

        public void When(SiteContentChanged e)
        {
            Content = e.Content;
        }

        public void When(SiteCreated c)
        {
            Identifier = c.AggregateIdentifier;

            Tenant = c.Tenant;
            Author = c.Author;

            Domain = c.Domain;
            Title = c.Title;
        }

        public void When(SiteDeleted _)
        {

        }

        public void When(SiteDomainChanged c)
        {
            Domain = c.Domain;
        }

        public void When(SiteTitleChanged c)
        {
            Title = c.Title;
        }

        public void When(SiteTypeChanged c)
        {
            
        }
    }
}
