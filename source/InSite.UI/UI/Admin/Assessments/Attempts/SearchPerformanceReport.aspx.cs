using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Domain.Organizations.PerformanceReport;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Admin.Assessments.Attempts
{
    public partial class SearchPerformanceReport : AdminBasePage
    {
        public class ReportItem
        {
            public string ReportId { get; set; }
            public bool CanDelete { get; set; }
            public bool CanEdit { get; set; }
            public string Name { get; set; }
            public string Language { get; set; }
            public string RequiredRole { get; set; }
            public string FileSuffix { get; set; }
            public decimal EmergentScore { get; set; }
            public decimal ConsistentScore { get; set; }
            public string NursingRoleText { get; set; }
            public string Description { get; set; }
            public ItemWeight[] Roles { get; set; }
            public ItemWeight[] AssessmentTypes { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportRepeater.ItemDataBound += ReportRepeater_ItemDataBound;
            ReportRepeater.ItemCommand += ReportRepeater_ItemCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var createItem = CanCreate
                    ? new BreadcrumbItem("Add New Report", "/ui/admin/assessments/attempts/create-performance-report")
                    : null;

                PageHelper.AutoBindHeader(this, createItem);

                BackButton.NavigateUrl = GetParentUrl(null);

                LoadData();
            }
        }

        private void ReportRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var report = (ReportItem)e.Item.DataItem;

            var roleRepeater = (Repeater)e.Item.FindControl("RoleRepeater");
            roleRepeater.DataSource = report.Roles;
            roleRepeater.DataBind();
        }

        private void ReportRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete")
                return;

            var reportId = Guid.Parse((string)e.CommandArgument);
            var report = TReportSearch.Select(reportId);

            TReportStore.Delete(reportId);

            LoadData();

            CommandAlert.AddMessage(AlertType.Success, $"Report '{report.ReportTitle}' has been deleted");
        }

        private void LoadData()
        {
            ReportRepeater.DataSource = GetReports();
            ReportRepeater.DataBind();
        }

        public static List<ReportItem> GetReports()
        {
            var reports = new List<ReportItem>();
            if (Organization.Toolkits.Assessments?.PerformanceReport?.Enabled != true)
                return reports;

            LoadOrganizationReports(reports);
            LoadSavedReports(reports);

            return reports;
        }

        private static void LoadOrganizationReports(List<ReportItem> reports)
        {
            for (int i = 0; i < Organization.Toolkits.Assessments.PerformanceReport.Reports.Length; i++)
            {
                var report = Organization.Toolkits.Assessments.PerformanceReport.Reports[i];

                reports.Add(new ReportItem
                {
                    ReportId = i.ToString(),
                    CanDelete = false,
                    CanEdit = false,
                    Name = report.Name,
                    Language = GetLanguageName(report.Language),
                    RequiredRole = report.RequiredRole,
                    Roles = report.RoleWeights.Select(x => x.Clone()).ToArray(),
                    FileSuffix = report.FileSuffix,
                    EmergentScore = report.EmergentScore,
                    ConsistentScore = report.ConsistentScore,
                    NursingRoleText = null,
                    Description = null,
                    AssessmentTypes = report.AssessmentTypeWeights.IsNotEmpty()
                        ? report.AssessmentTypeWeights.Select(x => x.Clone()).ToArray()
                        : Organization.Toolkits.Assessments.PerformanceReport.AssessmentTypeWeights.Select(x => x.Clone()).ToArray()
                });
            }
        }

        private static void LoadSavedReports(List<ReportItem> reports)
        {
            var savedReports = TReportSearch.Bind(
                x => x,
                x =>
                    x.OrganizationIdentifier == Organization.Identifier
                    && x.ReportType == TReport.Types.PerformanceReport
            );

            foreach (var savedReport in savedReports)
            {
                ReportItem report;

                if (int.TryParse(savedReport.ReportDescription, out var _))
                {
                    report = reports.Find(x => x.ReportId == savedReport.ReportDescription);
                    if (report == null)
                        continue;
                }
                else
                    reports.Add(report = new ReportItem { CanDelete = Identity.IsOperator, CanEdit = true });

                var data = JsonConvert.DeserializeObject<ReportVariant>(savedReport.ReportData);

                report.ReportId = savedReport.ReportIdentifier.ToString();
                report.Name = data.Name;
                report.Language = GetLanguageName(data.Language);
                report.RequiredRole = data.RequiredRole;
                report.FileSuffix = data.FileSuffix;
                report.EmergentScore = data.EmergentScore;
                report.ConsistentScore = data.ConsistentScore;
                report.NursingRoleText = data.NursingRoleText;
                report.Description = data.Description;
                report.Roles = data.RoleWeights;
                report.AssessmentTypes = data.AssessmentTypeWeights;
            }
        }

        private static string GetLanguageName(string language)
        {
            return language == "en"
                ? "English"
                : (language == "fr" ? "French" : throw new ArgumentException($"language: {language}"));
        }
    }
}