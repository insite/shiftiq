using System;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations.PerformanceReport;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Attempts
{
    public partial class EditPerformanceReport : AdminBasePage
    {
        private Guid? ReportId => Guid.TryParse(Request.QueryString["id"], out var id) ? id : (Guid?)null;

        private int? ReportIndex => int.TryParse(Request.QueryString["id"], out var id) ? id : (int?)null;

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

            var report = GetReport(ReportId, ReportIndex);

            if (report == null || !CanEdit)
                HttpResponseHelper.Redirect(GetParentUrl(null));

            PageHelper.AutoBindHeader(Page);

            CancelButton.NavigateUrl = GetParentUrl(null);

            Detail.SetInputValues(report);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var report = GetReport(ReportId, ReportIndex);

            Detail.GetInputValues(report);

            if (report.ReportIdentifier == Guid.Empty)
            {
                report.ReportIdentifier = UniqueIdentifier.Create();
                TReportStore.Insert(report);
            }
            else
                TReportStore.Update(report);

            HttpResponseHelper.Redirect(GetParentUrl(null));
        }

        public static ReportVariant GetReportData(string id)
        {
            var reportId = Guid.TryParse(id, out var temp1) ? temp1 : (Guid?)null;
            var reportIndex = int.TryParse(id, out var temp2) ? temp2 : (int?)null;

            var report = GetReport(reportId, reportIndex);

            return report != null
                ? JsonConvert.DeserializeObject<ReportVariant>(report.ReportData)
                : null;
        }

        private static TReport GetReport(Guid? reportId, int? reportIndex)
        {
            if (Organization.Toolkits.Assessments?.PerformanceReport?.Enabled != true)
                return null;

            if (reportId.HasValue)
                return TReportSearch.Select(reportId.Value);

            if (reportIndex == null || reportIndex < 0 || reportIndex >= Organization.Toolkits.Assessments.PerformanceReport.Reports.Length)
                return null;

            var report = TReportSearch.BindFirst(
                x => x,
                x =>
                    x.OrganizationIdentifier == Organization.Identifier
                    && x.ReportType == TReport.Types.PerformanceReport
                    && x.ReportDescription == reportIndex.ToString()
            );

            if (report != null)
                return report;

            var organizationReport = Organization.Toolkits.Assessments.PerformanceReport.Reports[reportIndex.Value].Clone();
            
            if (organizationReport.AssessmentTypeWeights.IsEmpty())
                organizationReport.AssessmentTypeWeights = Organization.Toolkits.Assessments.PerformanceReport.AssessmentTypeWeights;

            return new TReport
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                UserIdentifier = User.UserIdentifier,
                ReportType = TReport.Types.PerformanceReport,
                ReportTitle = organizationReport.Name,
                ReportDescription = reportIndex.ToString(),
                Created = DateTimeOffset.Now,
                CreatedBy = User.UserIdentifier,
                ReportData = JsonConvert.SerializeObject(organizationReport)
            };
        }
    }
}