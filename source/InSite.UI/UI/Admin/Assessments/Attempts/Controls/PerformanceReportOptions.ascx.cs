using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Toolbox.Reporting.PerformanceReport.Models;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class PerformanceReportOptions : BaseUserControl
    {
        private List<string> Reports
        {
            get => (List<string>)ViewState[nameof(Reports)];
            set => ViewState[nameof(Reports)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportValidator.ServerValidate += ReportValidator_ServerValidate;
        }

        private void ReportValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var reportIndex = GetSelectedReportIndex();
            var report = reportIndex >= 0 ? EditPerformanceReport.GetReportData(Reports[reportIndex]) : null;

            args.IsValid = report != null
                && !string.IsNullOrEmpty(report.RequiredRole)
                && report.RoleWeights != null
                && report.RoleWeights.Length > 0;
        }

        public void BindControls()
        {
            BindReports();

            SearchButton.HRef = "/ui/admin/assessments/attempts/search-performance-report";
        }

        public ReportConfig GetReportConfig(int? reportIndex = null)
        {
            if (reportIndex == null)
                reportIndex = GetSelectedReportIndex();

            var report = EditPerformanceReport.GetReportData(Reports[reportIndex.Value]);

            var roleWeights = report.RoleWeights
                .Select(role => new ItemWeight
                {
                    Name = role.Name,
                    Weight = role.Weight
                })
                .ToArray();

            var assessmentTypeWeights = report.AssessmentTypeWeights
                .Select(t => new ItemWeight
                {
                    Name = t.Name,
                    Weight = t.Weight
                })
                .ToArray();

            return new ReportConfig
            {
                Language = report.Language,
                FileSuffix = report.FileSuffix,
                EmergentScore = report.EmergentScore,
                ConsistentScore = report.ConsistentScore,
                RequiredRole = report.RequiredRole,
                RoleWeights = roleWeights,
                AssessmentTypeWeights = assessmentTypeWeights,
                NursingRoleText = report.NursingRoleText,
                Description = report.Description
            };
        }

        private void BindReports()
        {
            ReportRepeater.DataSource = ReadReports();
            ReportRepeater.DataBind();
        }

        private List<string> ReadReports()
        {
            var result = new List<string>();

            var reports = SearchPerformanceReport.GetReports();
            foreach (var report in reports)
            {
                var roleWeights = report.Roles != null
                    ? report.Roles.Select(role => $"{role.Name} {role.Weight:p0}").ToList()
                    : null;

                var rolesText = string.Join(", ", roleWeights);

                var assessmentTypes = string.Join(", ", report.AssessmentTypes.Select(t => $"{t.Name} {t.Weight:p0}"));

                var reportText = $"{report.Name} {rolesText}; {assessmentTypes}";

                result.Add(reportText);
            }

            Reports = reports.Select(x => x.ReportId).ToList();

            return result;
        }

        private int GetSelectedReportIndex()
        {
            for (int i = 0; i < ReportRepeater.Items.Count; i++)
            {
                var repeaterItem = ReportRepeater.Items[i];
                var selected = (IRadioButton)repeaterItem.FindControl("Selected");

                if (selected.Checked)
                    return i;
            }
            return -1;
        }
    }
}