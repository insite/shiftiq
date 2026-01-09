using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Organizations.PerformanceReport;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Constant;

using Literal = System.Web.UI.WebControls.Literal;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class PerformanceReportDetail : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AssesmentTypeValidator.ServerValidate += AssesmentTypeValidator_ServerValidate;
        }

        private void AssesmentTypeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = CBAWeight.ValueAsInt.Value + SLAWeight.ValueAsInt.Value == 100;
        }

        public void SetDefaultInputValues()
        {
            Language.Value = "en";

            BindRoles(null);
        }

        public void SetInputValues(TReport report)
        {
            if (report == null)
                throw new ArgumentNullException("report");

            var data = JsonConvert.DeserializeObject<ReportVariant>(report.ReportData);

            BindRoles(data.RoleWeights);

            ReportName.Text = data.Name;
            Language.Value = data.Language;
            RequiredRole.Value = data.RequiredRole;
            EmergentScore.ValueAsInt = (int)(data.EmergentScore * 100);
            ConsistentScore.ValueAsInt = (int)(data.ConsistentScore * 100);
            CBAWeight.ValueAsInt = (int)((data.AssessmentTypeWeights.FirstOrDefault(x => x.Name == "CBA")?.Weight ?? 0) * 100);
            SLAWeight.ValueAsInt = (int)((data.AssessmentTypeWeights.FirstOrDefault(x => x.Name == "SLA")?.Weight ?? 0) * 100);

            NursingRoleText.Text = data.NursingRoleText;
            Description.Value = data.Description;
        }

        public void GetInputValues(TReport report)
        {
            if (report == null)
                throw new ArgumentNullException("report");

            var data = new ReportVariant
            {
                Name = ReportName.Text,
                Language = Language.Value,
                RequiredRole = RequiredRole.Value,
                EmergentScore = (decimal)EmergentScore.ValueAsInt / 100m,
                ConsistentScore = (decimal)ConsistentScore.ValueAsInt / 100m,

                AssessmentTypeWeights = new ItemWeight[]
                {
                    new ItemWeight { Name = "CBA", Weight = (decimal)CBAWeight.ValueAsInt / 100m },
                    new ItemWeight { Name = "SLA", Weight = (decimal)SLAWeight.ValueAsInt / 100m }
                },

                NursingRoleText = NursingRoleText.Text,
                Description = Description.Value,
                RoleWeights = GetRoles(),
                FileSuffix = GetFileSuffix()
            };

            report.ReportTitle = data.Name;
            report.ReportData = JsonConvert.SerializeObject(data);
            report.Modified = DateTimeOffset.UtcNow;
            report.ModifiedBy = User.Identifier;
        }

        private ItemWeight[] GetRoles()
        {
            var result = new List<ItemWeight>();

            foreach (RepeaterItem item in RoleRepeater.Items)
            {
                var nameLiteral = (Literal)item.FindControl("Name");
                var weightInput = (NumericBox)item.FindControl("Weight");

                if (weightInput.ValueAsInt.HasValue)
                {
                    result.Add(new ItemWeight
                    {
                        Name = nameLiteral.Text,
                        Weight = (decimal)weightInput.ValueAsInt.Value / 100m
                    });
                }
            }

            return result.ToArray();
        }

        private string GetFileSuffix()
        {
            var role = RequiredRole.Value.ToLower();

            if (Language.Value == "en")
                return role;

            switch (role)
            {
                case "hca":
                    return "pas";
                case "lpn":
                    return "iaa";
                case "rn":
                    return "ia";
                default:
                    return role;
            }
        }

        private void BindRoles(ItemWeight[] roles)
        {
            var reportingTags = TCollectionItemCache
                .Query(new TCollectionItemFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    CollectionName = CollectionName.Standards_Organizations_Classification_Flag
                })
                .Where(x => x.ItemFolder == "Reporting Tags")
                .Select(x => new
                {
                    Name = x.ItemName,
                    Weight = roles != null && roles.Any(y => y.Name == x.ItemName)
                        ? (int)(roles.FirstOrDefault(y => y.Name == x.ItemName).Weight * 100)
                        : (int?)null
                })
                .ToList();

            foreach (var reportingTag in reportingTags)
                RequiredRole.Items.Add(new ComboBoxOption(reportingTag.Name, reportingTag.Name));

            RoleRepeater.DataSource = reportingTags;
            RoleRepeater.DataBind();
        }
    }
}