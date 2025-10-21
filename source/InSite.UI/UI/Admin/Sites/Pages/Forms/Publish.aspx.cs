using System;

using InSite.Application.Pages.Write;
using InSite.Application.Sites.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Sites.Pages
{
    public partial class Publish : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties
        private Guid PageId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;
        private QPage Entity => _entity ?? (_entity = ServiceLocator.PageSearch.GetPage(PageId));
        private string FinderRelativePath => "/ui/admin/sites/pages/search";
        private string OutlineUrl
            => $"/ui/admin/sites/pages/outline?id={PageId}&panel=setup";

        #endregion

        #region Fields

        private QPage _entity;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PublishButton.Click += PublishButton_Click;
            UnpublishButton.Click += UnpublishButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Entity == null || Entity.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect(FinderRelativePath);

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{Entity.Title} <span class='form-text'>{Entity.PageType}</span>");

            UnpublishButton.Visible = !Entity.IsHidden;
            PublishButton.Visible = Entity.IsHidden;

            PageDetails.BindPage(Entity);

            CancelButton.NavigateUrl = $"/ui/admin/sites/pages/outline?id={PageId}&panel=setup";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={PageId}&panel=setup"
                : null;
        }

        #endregion

        private void PublishButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new ChangePageVisibility(PageId, false));

            RedirectToOutline();
        }

        private void UnpublishButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new ChangePageVisibility(PageId, true));

            RedirectToOutline();
        }

        private void RedirectToSearch()
           => HttpResponseHelper.Redirect(OutlineUrl, true);

        private void RedirectToOutline()
            => HttpResponseHelper.Redirect(OutlineUrl, true);
    }
}