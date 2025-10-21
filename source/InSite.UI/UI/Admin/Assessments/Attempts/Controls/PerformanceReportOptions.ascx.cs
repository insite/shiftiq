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
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportValidator.ServerValidate += ReportValidator_ServerValidate;
        }

        private void ReportValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var reportIndex = GetSelectedReportIndex();
            var report = reportIndex >= 0 ? Organization.Toolkits.Assessments.PerformanceReport.Reports[reportIndex] : null;

            args.IsValid = report != null
                && !string.IsNullOrEmpty(report.RequiredRole)
                && report.RoleWeights != null
                && report.RoleWeights.Length > 0;
        }

        public void BindControls()
        {
            BindReports();
            BindAssessmentTypes();
        }

        public ReportConfig GetReportConfig(int? reportIndex = null)
        {
            if (reportIndex == null)
                reportIndex = GetSelectedReportIndex();

            var report = Organization.Toolkits.Assessments.PerformanceReport.Reports[reportIndex.Value];

            var roleWeights = report.RoleWeights
                .Select(role => new ItemWeight
                {
                    Name = role.Name,
                    Weight = role.Weight
                })
                .ToArray();

            var assessmentTypeWeights = GetAssessmentTypes();

            return new ReportConfig
            {
                Language = report.Language,
                FileSuffix = report.FileSuffix,
                EmergentScore = report.EmergentScore,
                ConsistentScore = report.ConsistentScore,
                RequiredRole = report.RequiredRole,
                RoleWeights = roleWeights,
                AssessmentTypeWeights = assessmentTypeWeights,
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

            var reports = Organization.Toolkits.Assessments?.PerformanceReport?.Reports;
            if (reports == null)
                return result;

            foreach (var report in reports)
            {
                var roleWeights = report?.RoleWeights != null
                    ? report.RoleWeights.Select(role => $"{role.Name} {role.Weight:p0}").ToList()
                    : null;

                var rolesText = string.Join(", ", roleWeights);

                var reportText = $"{report.Name} {rolesText}";

                result.Add(reportText);
            }

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

        class AssessmentTypeWeight
        {
            public string Name { get; set; }
            public int Weight { get; set; }
        }

        private void BindAssessmentTypes()
        {
            AssessmentTypeRepeater.DataSource = ReadAssessmentTypes();
            AssessmentTypeRepeater.DataBind();
        }

        private List<AssessmentTypeWeight> ReadAssessmentTypes()
        {
            var assessmentTypes = Organization.Toolkits.Assessments?.PerformanceReport?.AssessmentTypeWeights;
            return assessmentTypes != null
                ? assessmentTypes
                    .Select(type => new AssessmentTypeWeight
                    {
                        Name = type.Name,
                        Weight = (int)(type.Weight * 100)
                    }).ToList()
                : new List<AssessmentTypeWeight>();
        }

        private ItemWeight[] GetAssessmentTypes()
        {
            var result = new List<ItemWeight>();
            var assessmentTypes = Organization.Toolkits.Assessments?.PerformanceReport?.AssessmentTypeWeights;

            for (int i = 0; i < AssessmentTypeRepeater.Items.Count; i++)
            {
                var repeaterItem = AssessmentTypeRepeater.Items[i];
                var weightNumericBox = (NumericBox)repeaterItem.FindControl("Weight");

                var assessmentType = new ItemWeight
                {
                    Name = assessmentTypes[i].Name,
                    Weight = weightNumericBox.ValueAsInt.Value / 100m
                };
                result.Add(assessmentType);
            }

            return result.ToArray();
        }
    }
}