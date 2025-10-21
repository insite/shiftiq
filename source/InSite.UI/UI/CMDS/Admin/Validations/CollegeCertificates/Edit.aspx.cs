using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Profile.Employee.Certificate
{
    public partial class Edit : AdminBasePage, ICmdsUserControl
    {
        private const string SearchUrl = "/ui/cmds/admin/validations/college-certificates/search";
        private const string CreateUrl = "/ui/cmds/admin/validations/college-certificates/create";
        private const string SelfUrl = "/ui/cmds/admin/validations/college-certificates/edit";

        private Guid EmployeeIdentifier => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        private Guid CertificateIdentifier => Guid.TryParse(Request.QueryString["certificateID"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            LoadData();

            DeleteButton.Visible = Access.Delete;
            CloseButton.NavigateUrl = SearchUrl;
        }

        private void LoadData()
        {
            var entity = TCollegeCertificateSearch.Select(EmployeeIdentifier, CertificateIdentifier);
            if (entity == null)
                HttpResponseHelper.Redirect(SearchUrl, true);

            PageHelper.AutoBindHeader(
                this,
                Access.Create ? new BreadcrumbItem("Add New College Certificate", CreateUrl) : null);

            Details.SetInputValues(entity);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var info = TCollegeCertificateSearch.Select(EmployeeIdentifier, CertificateIdentifier);
            Details.GetInputValues(info);

            TCollegeCertificateStore.Update(info);

            if (EmployeeIdentifier == info.LearnerIdentifier && CertificateIdentifier == info.ProfileIdentifier)
            {
                LoadData();

                SetStatus(ScreenStatus, StatusType.Saved);

                return;
            }

            var url = $"{SelfUrl}?id={info.LearnerIdentifier}&certificateID={info.ProfileIdentifier}&status=saved";

            HttpResponseHelper.Redirect(url, true);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var entity = TCollegeCertificateSearch.Select(EmployeeIdentifier, CertificateIdentifier);

            TCollegeCertificateStore.Delete(entity.CertificateIdentifier);

            HttpResponseHelper.Redirect(SearchUrl);
        }
    }
}