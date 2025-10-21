using System;
using System.Web.UI;

using InSite.Application.Courses.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Learning.Catalogs
{
    public partial class Create : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/learning/catalogs/create";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CancelButton.NavigateUrl = Search.NavigateUrl;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var catalog = new TCatalog
            {
                OrganizationIdentifier = Organization.Identifier,
                CatalogIdentifier = UniqueIdentifier.Create(),
                CatalogName = CatalogName.Text,
            };

            var store = new Course1Store();
            store.InsertCatalog(catalog);

            HttpResponseHelper.Redirect($"{Edit.NavigateUrl}?id={catalog.CatalogIdentifier}");
        }
    }
}