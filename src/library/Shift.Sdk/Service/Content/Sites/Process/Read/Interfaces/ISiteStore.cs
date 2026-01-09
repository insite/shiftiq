using System;

using InSite.Domain.Sites.Sites;

namespace InSite.Application.Sites.Read
{
    public interface ISiteStore
    {
        void InsertSite(SiteCreated change);

        void UpdateSite(SiteContentChanged site);
        void UpdateSite(SiteTitleChanged site);
        void UpdateSite(SiteDomainChanged site);
        void UpdateSite(SiteConfigurationChanged site);
        void UpdateSite(SiteTypeChanged site);

        void DeleteAll();
        void DeleteOne(Guid site);
        void DeleteSite(SiteDeleted change);
    }
}
