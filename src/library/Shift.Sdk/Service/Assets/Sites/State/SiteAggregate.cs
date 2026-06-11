using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Sites.Sites
{
    public class SiteAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new SiteState();
        
        internal SiteState Data => (SiteState)State;

        public void CreateSite(Guid organization, Guid author, string type, string domain, string title, string configuration)
        {
            Apply(new SiteCreated(organization, author, type, domain, title, configuration));
        }

        public void ChangeSiteContent(ContentContainer content)
        {
            if (!ContentContainer.IsEqual(Data.Content, content))
                Apply(new SiteContentChanged(content));
        }

        public void ChangeSiteTitle(string title)
        {
            Apply(new SiteTitleChanged(title));
        }

        public void ChangeSiteDomain(string domain)
        {
            Apply(new SiteDomainChanged(domain));
        }

        public void ChangeSiteType(string type)
        {
            Apply(new SiteTypeChanged(type));
        }

        public void ChangeSiteConfiguration(string configuration)
        {
            Apply(new SiteConfigurationChanged(configuration));
        }

        public void DeleteSite()
        {
            Apply(new SiteDeleted());
        }
    }
}