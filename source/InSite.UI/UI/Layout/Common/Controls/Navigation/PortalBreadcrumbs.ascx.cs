using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Foundations;

using Shift.Common;
using Shift.Contract;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Portal.Controls
{
    public partial class PortalBreadcrumbs : Lobby.LobbyBaseControl
    {
        public string RootHref { get; set; } = "/ui/portal/home";
        public string RootText { get; set; } = "Portal";

        private string PageTitle
        {
            get
            {
                return (string)ViewState[nameof(PageTitle)];
            }
            set
            {
                ViewState[nameof(PageTitle)] = value;
                Page.Title = value;
            }
        }

        protected static ISecurityFramework Identity => CurrentSessionState.Identity;

        private List<BreadcrumbItem> _items = new List<BreadcrumbItem>();
        public ICollection<BreadcrumbItem> Items => _items;

        protected override void OnPreRender(EventArgs e)
        {
            if (PageTitle.IsNotEmpty())
                Page.Title = PageTitle;

            base.OnPreRender(e);
        }

        public void BindRoot()
        {
            RootItem.HRef = RootHref;
            RootItem.InnerText = Translate(RootText);
        }

        public void BindTitle(string qualifier = null)
        {
            string helpUrl = null;

            if (Page is PortalBasePage portalPage)
            {
                var title = GetDisplayText(portalPage.ActionModel.ActionName);

                if (portalPage is IHasTitle p)
                    title = p.GetTitle();

                PageTitle = (qualifier ?? title) + " - " + ServiceLocator.Partition.GetPlatformName();

                ActionTitle.Text = qualifier ?? title;

                ActionSubtitle.InnerText = string.Empty;

                helpUrl = portalPage.ActionModel.HelpUrl;
            }

            ActionHelp.Visible = !string.IsNullOrEmpty(helpUrl);
            ActionHelp.HRef = ServiceLocator.Urls.HelpUrl + helpUrl;

            if (ActionHelp.Visible)
                ActionHelpTitle.Text = Translate("Help");

            ActionSubtitle.Visible = !string.IsNullOrEmpty(ActionSubtitle.InnerText);
        }

        public void BindTitleAndSubtitleNoTranslate(string title, string subtitle) =>
            BindTitleAndSubtitleNoTranslate(title, title, subtitle);

        public void BindTitleAndSubtitleNoTranslate(string pageTitle, string screenTitle, string screenSubtitle)
        {
            if (Page is PortalBasePage)
                PageTitle = pageTitle + " - " + ServiceLocator.Partition.GetPlatformName();

            ActionTitle.Text = screenTitle;
            ActionSubtitle.InnerHtml = screenSubtitle;
            ActionSubtitle.Visible = !string.IsNullOrWhiteSpace(screenSubtitle);
        }

        public void BindTitleAndSubtitle(string title, string subtitle) =>
            BindTitleAndSubtitle(title, title, subtitle);

        public void BindTitleAndSubtitle(string pageTitle, string screenTitle, string screenSubtitle)
        {
            if (Page is PortalBasePage)
                PageTitle = pageTitle + " - " + ServiceLocator.Partition.GetPlatformName();

            ActionTitle.Text = GetDisplayText(screenTitle);
            ActionSubtitle.InnerHtml = GetDisplayText(screenSubtitle);
            ActionSubtitle.Visible = !string.IsNullOrWhiteSpace(screenSubtitle);
        }

        public void BindBreadcrumbs(BreadcrumbItem[] breadcrumbs)
        {
            BindBreadcrumbs(breadcrumbs, null);
        }

        public void BindBreadcrumbs(BreadcrumbItem[] breadcrumbs, BreadcrumbItem[] creates)
        {
            _items = breadcrumbs.ToList();

            BindRoot();
            BreadcrumbRepeater.DataSource = breadcrumbs;
            BreadcrumbRepeater.DataBind();
            BindCreate(creates);
        }

        public void BindCreate(BreadcrumbItem create)
        {
            if (create != null)
            {
                AddNewItem.Visible = true;
                AddNewAnchor.Visible = true;
                AddNewAnchor.HRef = create.Href;
                AddNewAnchor.InnerHtml = "<i class=\"fas fa-plus-circle ms-2 me-1\"></i>" + create.Text;
            }
        }

        public void BindCreate(BreadcrumbItem[] creates)
        {
            if (creates == null)
                return;

            var anchors = new List<BreadcrumbItem>();
            foreach (var create in creates)
            {
                if (create?.Href == null || !create.Href.StartsWith("/"))
                    continue;

                var actionUrl = create.Href.Substring(1);
                if (Identity.IsActionAuthorized(actionUrl))
                    anchors.Add(create);
            }

            if (anchors.Count == 0)
                return;

            AddNewList.Visible = true;
            AddNewAnchors.DataSource = anchors;
            AddNewAnchors.DataBind();
        }

        public void HideBreadcrumb()
            => Breadcrumb.Visible = false;

        public void HideTitle()
            => TitlePanel.Visible = false;
    }
}