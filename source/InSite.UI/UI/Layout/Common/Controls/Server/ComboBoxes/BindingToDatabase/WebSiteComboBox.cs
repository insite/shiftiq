using System.Linq;

using InSite.Application.Sites.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Common.Web.UI
{
    public class WebSiteComboBox : ComboBox
    {
        public QSiteFilter ListFilter => (QSiteFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new QSiteFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            ListFilter.OrderBy = "SiteDomain";

            var data = ServiceLocator.SiteSearch.Bind(x => x, ListFilter);
            foreach (var item in data)
                list.Add(item.SiteIdentifier.ToString(), item.SiteDomain);

            return list;
        }
    }

    public class WebFolderComboBox : ComboBox
    {
        public SitemapFilter ListFilter => (SitemapFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = CreateDefaultFilter()));

        private static SitemapFilter CreateDefaultFilter() => new SitemapFilter
        {
            OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
            PageType = "Folder"
        };

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var data = SitemapSearch.Get(ListFilter).OrderBy(x => x.PathSequence);
            foreach (var item in data)
                list.Add(item.PageIdentifier.ToString(), item.PageSlug);

            return list;
        }
    }

    public class WebPageComboBox : ComboBox
    {
        public SitemapFilter ListFilter => (SitemapFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = CreateDefaultFilter()));

        private static SitemapFilter CreateDefaultFilter() => new SitemapFilter
        {
            OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
            PageType = "Page"
        };

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var data = SitemapSearch.Get(ListFilter).OrderBy(x => x.PageSlug).ToArray();
            foreach (var item in data)
                list.Add(item.PageIdentifier.ToString(), item.PageSlug);

            return list;
        }
    }
}