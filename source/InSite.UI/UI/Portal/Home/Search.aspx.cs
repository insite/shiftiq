using System;
using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Home
{
    public partial class Search : PortalBasePage
    {
        private string SearchQuery => Page.Request.QueryString["query"].EmptyIfNull().Trim();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchText.FilterClick += (x, y) => RedirectToSearch();
            SearchText.ClearClick += (x, y) => RedirectToSearch();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                SearchText.Text = SearchQuery;

                LoadSearchResults();
            }
        }

        private void RedirectToSearch()
        {
            var url = $"/ui/portal/home/search?query={HttpUtility.UrlEncode(SearchText.Text)}";
            HttpResponseHelper.Redirect(url);
        }

        private void LoadSearchResults()
        {
            var helpPages = PageSearch.GetOrgHelpPages(Organization.Identifier, SearchQuery);
            var all = helpPages.Select(x => x.WebPageIdentifier).ToHashSet();

            var containers = helpPages.Select(x => x.WebPageIdentifier).ToList();

            var permissions = TGroupPermissionSearch.Select(x => containers.Any(y => y == x.ObjectIdentifier)).ToList();

            var allowed = TGroupPermissionSearch.GetAccessAllowed(all, Identity, permissions);

            var allowedHelpPages = helpPages
                .Where(x => allowed.Contains(x.WebPageIdentifier))
                .ToList();

            foreach (var helpPage in allowedHelpPages)
                helpPage.ContentSnip = ContentContainerItem.GetSnip(helpPage.ContentText, helpPage.ContentHtml, 300);

            ResultRepeater.DataSource = allowedHelpPages;
            ResultRepeater.DataBind();

            BindCount(allowedHelpPages.Count);
        }

        private void BindCount(int count)
        {
            var pagesText = count == 1 ? "page" : "pages";

            FoundLiteral.Text = $"Found {count} {pagesText}";

            FoundDiv.Visible = count > 0;
            NoResults.Visible = count == 0;
            ResultRepeater.Visible = count > 0;
        }

        protected string GetPageUrl()
        {
            var record = (HelpPageRecord)Page.GetDataItem();
            string url;

            if (string.Equals(record.WebPageType, "Block", StringComparison.OrdinalIgnoreCase))
            {
                var index = record.PathUrl.LastIndexOf('/');
                url = index > 0 ? record.PathUrl.Substring(0, index) : record.PathUrl;
            }
            else
                url = record.PathUrl;

            return $"/portals/{url}";
        }
    }
}