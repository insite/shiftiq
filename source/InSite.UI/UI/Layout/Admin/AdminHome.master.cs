using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Admin.Sites.Utilities;
using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Layout.Admin
{
    public partial class AdminHome : MasterPage
    {
        public Common.Controls.Navigation.AdminHeaderControl AdminHeader => AdminHeaderControl;
        public Common.Controls.Navigation.Navigator Navigator { get; private set; }

        protected ISecurityFramework Identity => CurrentSessionState.Identity;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Navigator = new Common.Controls.Navigation.Navigator(Request);
            AdminHeader.Navigator = Navigator;

            HelpColumnOrganizationCreate1.Click += (x, y) => CreateCustomHelpPage(Request.Url.AbsolutePath);
            HelpColumnOrganizationCreate2.Click += (x, y) => CreateCustomHelpPage(Request.Url.AbsolutePath);
        }

        private void CreateCustomHelpPage(string url)
        {
            var webPageIdentifier = SiteHelper.CreateCustomHelpPage(url, Identity.User, Identity.Organization, true);
            Response.Redirect($"/ui/admin/sites/pages/outline?id={webPageIdentifier}&panel=content");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            HttpResponseHelper.SetNoCache();
        }

        public void RenderHelpContent(TAction model)
        {
            BindOrganizationHelp(model, HelpColumnOrganizationTitle1, HelpColumnOrganizationBody1, HelpColumnOrganizationCreate1, HelpColumnOrganizationEdit1);
            BindOrganizationHelp(model, HelpColumnOrganizationTitle2, HelpColumnOrganizationBody2, HelpColumnOrganizationCreate2, HelpColumnOrganizationEdit2);

            BindHelpLink();
        }

        protected void BindPlatformHelp(TAction action, HtmlGenericControl heading, HtmlGenericControl body, HyperLink edit)
        {
            edit.Visible = false;

            if (Navigator.IsCmds)
                body.InnerHtml = "<p>Need support? Please check our updated user guides under Help.</p>";

            if (action != null)
            {
                edit.Target = "_blank";
                edit.NavigateUrl = $"/ui/admin/platform/routes/edit?id={action.ActionIdentifier}&panel=content";
                edit.Text = "<i class='far fa-edit me-1'></i>Edit</a>";
                edit.Visible = Identity.IsOperator;
            }
        }

        protected void BindOrganizationHelp(TAction action, HtmlGenericControl heading, HtmlGenericControl body, LinkButton create, HyperLink edit)
        {
            heading.InnerText = Identity.Organization.Name;

            create.Text = "<i class='far fa-edit me-1'></i>Edit</a>";
            edit.Text = "<i class='far fa-edit me-1'></i>Edit</a>";

            var pageId = GetHelpPageId(action.ActionUrl);
            var bodyHtml = string.Empty;

            if (pageId != null)
            {
                var contents = ServiceLocator.ContentSearch.SelectContainerByLabel(pageId.Value, "Body");
                var content = contents.FirstOrDefault(x => x.ContentLanguage == "en");
                if (content?.ContentText != null)
                    bodyHtml = Markdown.ToHtml(content.ContentText);
            }

            var hasContent = pageId != null && !string.IsNullOrWhiteSpace(bodyHtml);

            if (!hasContent)
            {
                create.Visible = true;
                edit.Visible = false;

                BodyColumn.Attributes["class"] = "col-lg-12";
                HelpColumn.Attributes["class"] = "d-none";

                return;
            }

            body.InnerHtml = bodyHtml;

            edit.Target = "_blank";
            edit.NavigateUrl = $"/ui/admin/sites/pages/outline?id={pageId}&panel=content";
            edit.Text = "<i class='far fa-edit me-1'></i>Edit</a>";
            edit.Visible = true;

            create.Visible = false;
        }

        private void BindHelpLink()
        {
            HomeHelpLink.HRef = ServiceLocator.Urls.HelpUrl + "/help";

            if (Page is PortalBasePage p)
            {
                if (p?.ActionModel?.HelpUrl != null)
                {
                    HomeHelpLink.HRef = ServiceLocator.Urls.HelpUrl + p.ActionModel.HelpUrl;

                    HelpColumnPlatformTitle2.InnerText = p.ActionModel.ActionName;
                    HelpColumnPlatformBody2.InnerHtml = $"Click the <a target='_blank' href='{HomeHelpLink.HRef}'>help topic link here</a> for more documentation about this page.";
                }
                else if (p?.ActionModel != null)
                {
                    BindPlatformHelp(p?.ActionModel, HelpColumnPlatformTitle2, HelpColumnPlatformBody2, HelpColumnPlatformEdit2);
                }
            }
        }

        internal static Guid? GetHelpPageId(string actionUrl)
        {
            var organizationId = CurrentSessionState.Identity.Organization.Identifier;

            var pages = ServiceLocator.PageSearch.Bind(
                x => x.PageIdentifier,
                x => x.OrganizationIdentifier == organizationId && x.Parent.PageSlug == "in-help" && x.PageSlug == actionUrl.Replace("/", "-"));

            return pages.Length == 1 ? pages[0] : (Guid?)null;
        }

        public void BindThemeMode(string mode)
        {
            DarkStyle.Visible = mode == "Dark";
        }

        public void HideHelpColumn()
        {
            BodyColumn.Attributes["class"] = "col-lg-12";
            HelpColumn.Attributes["class"] = "d-none";
        }
    }
}