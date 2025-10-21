using System;

using InSite.Admin.Sites.Utilities;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Sites.Pages;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Sites.Pages
{
    public partial class ChangeStructure : AdminBasePage, IHasParentLinkParameters
    {
        private Guid PageId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            WebSiteSelector.AutoPostBack = true;
            WebSiteSelector.ValueChanged += WebSiteSelector_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var page = ServiceLocator.PageSearch.GetPage(PageId);

                if (page == null || page.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/sites/pages/search");
                    return;
                }

                PageHelper.AutoBindHeader(this, null, page.Title);

                PageDetails.BindPage(page);

                ParentPageId.SiteId = page.SiteIdentifier;
                ParentPageId.Value = page.ParentPageIdentifier;
                WebSiteSelector.ValueAsGuid = page.SiteIdentifier;
                PageSlug.Text = page.PageSlug;
                NavigateUrl.Text = page.NavigateUrl;
                IsNavigateUrlToNewTab.Checked = page.IsNewTab;

                CancelButton.NavigateUrl = $"/ui/admin/sites/pages/outline?id={PageId}&panel=setup";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var commands = new PageCommandGenerator().
                GetDifferencePageSetupCommands(
                    GetEntityValues(),
                    GetInputValues()
                );

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect($"/ui/admin/sites/pages/outline?id={PageId}&panel=setup");
        }

        private void WebSiteSelector_ValueChanged(object sender, EventArgs e)
        {
            var siteId = WebSiteSelector.ValueAsGuid;

            ParentPageId.SiteId = siteId;

            if (!ParentPageId.Value.HasValue)
                return;

            var parentId = ParentPageId.Value.Value;

            if (!ServiceLocator.PageSearch.Exists(x => x.SiteIdentifier == siteId && x.PageIdentifier == parentId))
                ParentPageId.Value = null;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={PageId}&panel=setup"
                : null;
        }

        private PageState GetEntityValues()
        {
            var page = ServiceLocator.PageSearch.GetPage(PageId);
            return new PageState()
            {
                Identifier = page.PageIdentifier,
                ParentPage = page.ParentPageIdentifier,
                Site = page.SiteIdentifier
            };
        }

        private PageState GetInputValues()
        {
            return new PageState()
            {
                ParentPage = ParentPageId.Value,
                Site = WebSiteSelector.ValueAsGuid,
                Slug = SiteHelper.SanitizeSiteName(PageSlug.Text),
                NavigateUrl = NavigateUrl.Text,
                IsNewTab = IsNavigateUrlToNewTab.Checked
            };
        }
    }
}