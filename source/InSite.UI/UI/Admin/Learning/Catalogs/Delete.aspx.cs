using System;

using InSite.Application.Courses.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Learning.Catalogs
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        public const string NavigateUrl = "/ui/admin/learning/catalogs/delete";

        private Guid CatalogIdentifier => Guid.TryParse(Request.QueryString["catalog"], out var catalog) ? catalog : Guid.Empty;

        private TCatalog Model { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CancelButton.NavigateUrl = Search.NavigateUrl;

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();

            PageHelper.AutoBindHeader(Page, null, Model.CatalogName);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var store = new Course1Store();

            store.DeleteCatalog(CatalogIdentifier);

            HttpResponseHelper.Redirect(CancelButton.NavigateUrl);
        }

        private void LoadData()
        {
            var catalog = CourseSearch.SelectCatalog(CatalogIdentifier);
            if (catalog == null || catalog.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(CancelButton.NavigateUrl);

            Model = catalog;

            CatalogName.Text = $"<a href='{Edit.NavigateUrl}?catalog={catalog.CatalogIdentifier}'>{catalog.CatalogName}</a>";
        }

        IWebRoute IOverrideWebRouteParent.GetParent()
            => GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => GetParentLinkParameters(parent, null);
    }
}