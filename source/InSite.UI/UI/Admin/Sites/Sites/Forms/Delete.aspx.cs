using System;

using InSite.Application.Pages.Write;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Sites.Sites.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid SiteIdentifier => Guid.TryParse(Request["id"], out var asset) ? asset : Guid.Empty;

        private QSite Entity => _entity ?? (_entity = ServiceLocator.SiteSearch.Select(SiteIdentifier));

        private string EditorRelativePath
        {
            get
            {
                var parameters = GetEditorParameters();
                return HttpResponseHelper.BuildUrl("/ui/admin/sites/outline", parameters);
            }
        }

        private string FinderRelativePath => "/ui/admin/sites/sites/search";

        #endregion

        #region Fields

        private QSite _entity;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;

            CancelButton.NavigateUrl = EditorRelativePath;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Entity == null || Entity.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect(EditorRelativePath);

            PageHelper.AutoBindHeader(this, null, Entity.SiteTitle);

            SiteDetails.BindSite(Entity);

            var containedPagesCount = ServiceLocator.PageSearch.Count(new QPageFilter { WebSiteIdentifier = Entity.SiteIdentifier });
            ContainedPagesCount.Text = $"{containedPagesCount:n0}";
        }

        #endregion

        #region Event handlers

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            var pages = ServiceLocator.PageSearch.Bind(x => x.PageIdentifier, x => x.SiteIdentifier == Entity.SiteIdentifier);
            foreach (var pageId in pages)
                ServiceLocator.SendCommand(new DeletePage(pageId));

            ServiceLocator.SendCommand(new DeleteSite(Entity.SiteIdentifier));

            HttpResponseHelper.Redirect(FinderRelativePath);
        }

        #endregion

        #region Methods (helpers)

        private string GetEditorParameters()
        {
            return $"id={SiteIdentifier}";
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? GetEditorParameters()
                : null;
        }

        #endregion
    }
}