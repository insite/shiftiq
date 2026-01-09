using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Admin.Sites.Utilities;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.UI.Layout.Lobby;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Layout.Portal
{
    public partial class PortalMaster : MasterPage
    {
        #region Properties

        public Controls.PortalBreadcrumbs Breadcrumbs => BodyBreadcrumbs;

        protected ISecurityFramework Identity => CurrentSessionState.Identity;

        public bool IsManagerGroupEnable
        {
            get
            {
                return Identity?.Organization?.Toolkits?.Sales?.ManagerGroup.HasValue == true;
            }
        }

        public bool IsManagerGroupMember
        {
            get
            {
                if (!IsAuthenticated)
                    return false;

                if (!IsManagerGroupEnable)
                    return false;

                var groupId = Identity.Organization.Toolkits.Sales.ManagerGroup.Value;
                return Identity.IsInGroup(groupId);
            }
        }

        public bool IsLearnerGroupEnable
        {
            get
            {
                return Identity?.Organization?.Toolkits?.Sales?.LearnerGroup.HasValue == true;
            }
        }

        public bool IsLearnerGroupMember
        {
            get
            {
                if (!IsAuthenticated)
                    return false;

                if (!IsLearnerGroupEnable)
                    return false;

                var groupId = Identity.Organization.Toolkits.Sales.LearnerGroup.Value;
                return Identity.IsInGroup(groupId);
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return Identity.IsAuthenticated;
            }
        }

        public bool IsSalesReady => IsManagerGroupEnable && IsLearnerGroupEnable;

        #endregion


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CustomAboutCreate.Click += (x, y) => CreateCustomHelpPage(Request.RawUrl);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (IsPostBack)
                AntiForgeryHelper.Validate();

            base.OnLoad(e);
        }

        public void HideBreadcrumbsAndTitle()
            => BodyBreadcrumbs.Visible = false;

        public void HideBreadcrumbsOnly()
            => BodyBreadcrumbs.HideBreadcrumb();

        public void HideSideContent()
        {
            SideContent.Visible = false;
            HelpHeading.Attributes["class"] += " rounded-top";
        }

        public void OverrideHomeLink(string url)
        {
            PortalHeader.OverrideHomeLink(url);
            Breadcrumbs.RootHref = url;
        }

        public void UpdateHeaderCartBadge()
        {
            PortalHeader.BindCartBadge();
            HeaderUpdatePanel.Update();
        }

        public void SidebarVisible(bool visible)
        {
            SideColumn.Visible = visible;
        }

        public void EnableSidebarToggle(bool enable)
        {
            SidebarToggle.Visible = enable;
        }

        public void ShowAvatar(string dashboardUrl = null)
        {
            var identity = CurrentSessionState.Identity;

            if (identity.User == null)
                return;

            var avatar = identity.User;

            if (Guid.TryParse(Request.QueryString["learner"], out Guid id))
                avatar = UserSearch.SelectWebContact(id, identity.Organization.Identifier);

            var person = PersonSearch.Select(identity.Organization.OrganizationIdentifier, avatar.UserIdentifier);

            MyAvatar.AlternateText = avatar.FullName;
            MyName.InnerText = avatar.FullName;
            MyEmail.InnerText = avatar.Email;
            MyCode.InnerText = IsSalesReady ? string.Empty : person.PersonCode;
            MyCodeContainer.Visible = !IsSalesReady;
            MyAvatar.ImageUrl = ServiceLocator.AppSettings.Application.DefaultAvatarImageUrl;
            var image = UserSearch.Bind(avatar.Identifier, x => x.ImageUrl);
            if (image != null)
                MyAvatar.ImageUrl = image;

            MyAvatarContainer.Visible = true;
            NoAvatarContainer.Visible = false;

            if (dashboardUrl.IsNotEmpty())
            {
                MyAvatarLink.HRef = dashboardUrl;
                MyNameLink.HRef = dashboardUrl;
            }
        }

        protected string Translate(string input)
        {
            if (Page is LobbyBasePage)
                return ((LobbyBasePage)Page).GetDisplayText(input);
            return input;
        }

        public void RenderHelpContent(TAction model)
        {
            if (IsSalesReady)
            {
                HelpPanel.Visible = false;
                return;
            }

            if (model == null)
            {
                HelpPanel.Visible = false;
                return;
            }

            BindCustomAbout(model, CustomAboutHeading, CustomAboutBody, CustomAboutCreate, CustomAboutEdit);
        }

        private void BindCustomAbout(TAction action, HtmlGenericControl heading, HtmlGenericControl body, LinkButton create, HyperLink edit)
        {
            heading.InnerText = Translate("Administrators Only");
            body.InnerHtml = $"<p>{Translate("You can post your own help content for this page to give your learners extra guidance, specific to your organization.")}</p>";

            var allowEdit = Identity.IsGranted("Admin/Sites", PermissionOperation.Write);

            heading.Visible = allowEdit;
            body.Visible = allowEdit;

            create.Text = "<i class='fas fa-pencil me-1'></i>Edit</a>";
            edit.Text = "<i class='fas fa-pencil me-1'></i>Edit</a>";

            var page = SiteHelper.GetCustomHelpPage(Request.RawUrl, Identity.User, Identity.Organization, true);

            if (page == null)
                page = ServiceLocator.PageSearch.Bind(
                    x => x,
                    x => x.OrganizationIdentifier == Identity.Organization.Identifier && x.Parent.PageSlug == "in-help" && x.PageSlug == action.ActionUrl.Replace("/", "-")).FirstOrDefault();

            if (page == null)
            {
                create.Visible = allowEdit;
                edit.Visible = false;

                if (SideContent.Controls.Count == 0)
                {
                    SidebarVisible(false);
                }

                return;
            }

            var contents = ServiceLocator.ContentSearch.SelectContainerByLabel(page.PageIdentifier, "Body");
            var content = contents.FirstOrDefault(x => x.ContentLanguage == Identity.Language);
            if (content?.ContentText != null)
            {
                heading.InnerText = Identity.Organization.Name;
                heading.Visible = true;
                body.InnerHtml = Markdown.ToHtml(content.ContentText);
                body.Visible = true;
            }

            edit.Target = "_blank";
            edit.NavigateUrl = $"/ui/admin/sites/pages/outline?id={page.PageIdentifier}&panel=content";
            edit.Text = "<i class='fas fa-pencil me-1'></i>Edit</a>";
            edit.Visible = allowEdit;

            create.Visible = false;
        }

        private void CreateCustomHelpPage(string url)
        {
            var webPageIdentifier = SiteHelper.CreateCustomHelpPage(url, Identity.User, Identity.Organization, true);
            Response.Redirect($"/ui/admin/sites/pages/outline?id={webPageIdentifier}&panel=content");
        }
    }
}