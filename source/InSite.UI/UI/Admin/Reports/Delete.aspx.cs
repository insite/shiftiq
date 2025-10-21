using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Reports
{
    public partial class Delete : AdminBasePage
    {
        private Guid ReportID => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteCheck.AutoPostBack = true;
            DeleteCheck.CheckedChanged += (x, y) => { DeleteButton.Enabled = DeleteCheck.Checked; };

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = "/ui/admin/reports/search";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var report = VReportSearch.Select(ReportID);

            if (report == null)
                HttpResponseHelper.SendHttp404();

            if (report.OrganizationIdentifier != Organization.Identifier
                || !VReportSearch.HasPermissions(report, Organization.Identifier, User.UserIdentifier)
                )
            {
                HttpResponseHelper.SendHttp403();
            }

            ReportDetail.BindReport(report);

            PageHelper.AutoBindHeader(Page, null, report.ReportTitle);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            TReportStore.Delete(ReportID);

            HttpResponseHelper.Redirect("/ui/admin/reports/search");
        }
    }
}