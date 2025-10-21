using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

using IHasParentLinkParameters = Shift.Common.IHasParentLinkParameters;
using IWebRoute = Shift.Common.IWebRoute;

namespace InSite.UI.Admin.Reports
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? ReportIdentifier => Guid.TryParse(Request.QueryString["id"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            SetupDownloadSection();

            CancelLink.NavigateUrl = $"/ui/admin/reports/edit?id={ReportIdentifier}";
        }

        private void OnFileTypeChanged()
        {
            FileFormatSelector.Items.Clear();

            FileFormatField.Visible = true;
            FileFormatSelector.Items.Add(new ListItem(FileFormat.Json.Text, FileFormat.Json.Value) { Selected = true });
        }

        private void SetupDownloadSection()
        {
            OnFileTypeChanged();

            FileName.Text = string.Format("report-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);

            if (!ReportIdentifier.HasValue)
            {
                PageHelper.AutoBindHeader(this, null, null);

                return;
            }

            var report = VReportSearch.Select(ReportIdentifier.Value);
            if (report == null)
                HttpResponseHelper.SendHttp404();

            if (report.OrganizationIdentifier != Organization.Identifier
                || !VReportSearch.HasPermissions(report, Organization.Identifier, User.UserIdentifier)
                )
            {
                HttpResponseHelper.SendHttp403();
            }

            PageHelper.AutoBindHeader(this, null, report.ReportTitle);

            ReportDetail.BindReport(report);
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var fileFormat = FileFormatSelector.SelectedValue;
            if (fileFormat == "JSON")
                SendJson();
        }

        private void SendJson()
        {
            var data = VReportHelper.Serialize(
                 VReportSearch.Select(ReportIdentifier.Value));

            if (CompressionMode.Value == "ZIP")
                SendZipFile(data, FileName.Text, "json");
            else
                Response.SendFile(FileName.Text, "json", data);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"id={ReportIdentifier}"
                : null;
        }
    }
}