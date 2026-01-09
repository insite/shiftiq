using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Attempts
{
    public partial class CreatePerformanceReport : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Organization.Toolkits.Assessments?.PerformanceReport?.Enabled != true)
                HttpResponseHelper.Redirect("/");

            if (!CanCreate)
                HttpResponseHelper.Redirect(GetParentUrl(null));

            PageHelper.AutoBindHeader(Page);

            CancelButton.NavigateUrl = GetParentUrl(null);

            Detail.SetDefaultInputValues();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var report = new TReport
            {
                ReportIdentifier = UniqueIdentifier.Create(),
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                UserIdentifier = User.UserIdentifier,
                ReportType = TReport.Types.PerformanceReport,
                Created = DateTimeOffset.Now,
                CreatedBy = User.UserIdentifier,
            };

            Detail.GetInputValues(report);

            TReportStore.Insert(report);

            HttpResponseHelper.Redirect(GetParentUrl(null));
        }
    }
}