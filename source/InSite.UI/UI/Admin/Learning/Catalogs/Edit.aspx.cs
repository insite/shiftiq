using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Learning.Catalogs
{
    public partial class Edit : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/learning/catalogs/edit";

        private Guid CatalogIdentifier => Guid.TryParse(Request.QueryString["catalog"], out var catalog) ? catalog : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CancelButton.NavigateUrl = Search.NavigateUrl;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var catalog = CourseSearch.SelectCatalog(CatalogIdentifier);
            catalog.CatalogName = CatalogName.Text;
            catalog.IsHidden = CatalogIsHidden.Checked;

            var store = new Course1Store();
            store.UpdateCatalog(catalog);

            HttpResponseHelper.Redirect(CancelButton.NavigateUrl);
        }

        private void LoadData()
        {
            var catalog = CourseSearch.SelectCatalog(CatalogIdentifier);

            if (catalog == null || catalog.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(CancelButton.NavigateUrl);

            PageHelper.AutoBindHeader(this, null, catalog.CatalogName);

            CatalogName.Text = catalog.CatalogName;

            CatalogIsHidden.Checked = catalog.IsHidden;

            var courses = CourseSearch.GetCatalogCourseList(CatalogIdentifier, Identity, (hook, course) => null)
                .OrderBy(x => x.Title)
                .ToList();

            CourseRepeater.DataSource = courses;

            CourseRepeater.DataBind();

            var programs = ProgramSearch.GetPrograms(new TProgramFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                CatalogIdentifier = CatalogIdentifier,
            })
                .OrderBy(x => x.ProgramName)
                .ToList();

            ProgramRepeater.DataSource = programs;

            ProgramRepeater.DataBind();

            DeleteButton.NavigateUrl = $"{Delete.NavigateUrl}?catalog={CatalogIdentifier}";
        }
    }
}
