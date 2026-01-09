using System;
using System.Collections.Generic;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Contract;

namespace InSite.UI.Layout.Common.Controls.Navigation
{
    public partial class AdminHeaderControl : BaseUserControl
    {
        public Navigator Navigator { get; internal set; }

        private string PageTitle
        {
            get => (string)ViewState[nameof(PageTitle)];
            set => ViewState[nameof(PageTitle)] = value;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (PageTitle.IsNotEmpty())
                Page.Title = PageTitle;

            base.OnPreRender(e);

            if (Page.IsPostBack)
                return;

            RecentLinkCache.Add(Page);

            var environment = ServiceLocator.AppSettings.Environment;

            var name = environment.Name;

            var theme = environment.Color;

            var icon = environment.Icon;

            var badge = "<a href='/ui/portal/platform/environments'>" +
                $"<span class=\"badge bg-{theme} fs-sm\">{name}</span>" +
                "</a>";

            if (environment.IsProduction())
            {
                EnvironmentReminder.Attributes["class"] = $"float-end";

                EnvironmentReminder.InnerHtml = badge;
            }
            else
            {
                EnvironmentReminder.Attributes["class"] = $"float-end text-{theme}";

                EnvironmentReminder.InnerHtml =
                    "<small><i class='fa-solid fa-circle-info me-2'></i>" +
                    $"Remember you are <strong>not</strong> working in a live version</small> {badge}";
            }
        }

        public void BindTitle(string qualifier = null)
        {
            if (Page is Admin.AdminBasePage p)
            {
                PageTitle = p.ActionModel.ActionName;

                ActionTitle.InnerHtml = p.ActionModel.ActionName;

                if (qualifier != null)
                {
                    PageTitle = $"{p.ActionModel.ActionName} - {RemoveHtmlElements(qualifier)}";

                    var indicator = p.ActionModel.ActionUrl.EndsWith("/delete") || p.ActionModel.ActionUrl.Contains("/delete-")
                        ? "danger"
                        : "info";

                    ActionTitle.InnerHtml += $" - <span class='text-{indicator}'>{qualifier}</span>";
                }

                ActionSubtitle.InnerText = string.Empty;
            }
            else if (Page is Portal.PortalBasePage q)
            {
                PageTitle = (qualifier ?? q.ActionModel.ActionName) + " - " + ServiceLocator.Partition.GetPlatformName();

                ActionTitle.InnerHtml = qualifier ?? q.ActionModel.ActionName;

                ActionSubtitle.InnerText = string.Empty;
            }

            ActionSubtitle.Visible = !string.IsNullOrEmpty(ActionSubtitle.InnerText);

            string RemoveHtmlElements(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return input;

                string intermediateText = System.Text.RegularExpressions.Regex.Replace(input, "<span.*", "", System.Text.RegularExpressions.RegexOptions.Singleline);
                return System.Text.RegularExpressions.Regex.Replace(intermediateText, "<.*?>", "").Trim();
            }
        }

        public void DisplayCalendar()
        {
            GoToCalendar.Visible = true;
        }

        public void BindBreadcrumbs(BreadcrumbItem[] breadcrumbs, BreadcrumbItem create)
        {
            BindRoot(breadcrumbs);
            BreadcrumbRepeater.DataSource = breadcrumbs;
            BreadcrumbRepeater.DataBind();
            BindCreate(create);
        }

        public void BindBreadcrumbs(BreadcrumbItem[] breadcrumbs, BreadcrumbItem[] creates)
        {
            BindRoot(breadcrumbs);
            BreadcrumbRepeater.DataSource = breadcrumbs;
            BreadcrumbRepeater.DataBind();
            BindCreate(creates);
        }

        private void BindRoot(BreadcrumbItem[] breadcrumbs)
        {
            if (Navigator.IsCmds)
            {
                if (breadcrumbs != null && breadcrumbs.Length > 1)
                {
                    HomeLink.HRef = Urls.CmdsHomeUrl;
                    HomeLink.InnerText = "Home";
                    HomeItem.Visible = true;
                }
            }
            else
            {
                AdminLink.HRef = Urls.AdminHomeUrl;
                AdminLink.InnerText = "Admin";
                AdminItem.Visible = true;
            }
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
                if (create.Href == null || !create.Href.StartsWith("/"))
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

        public void BindSubtitle(string subtitle)
        {
            ActionSubtitle.InnerHtml = subtitle;
            ActionSubtitle.Visible = !string.IsNullOrEmpty(ActionSubtitle.InnerText);
        }

        public void HideBreadcrumbs()
        {
            Breadcrumbs.Visible = false;
        }

        public void OverrideTitle(string title)
        {
            ActionTitle.InnerHtml = title;
        }

        public void HideTitle()
        {
            ActionTitlePanel.Visible = false;
            HiddenTitleLiteral.Visible = true;
        }
    }
}