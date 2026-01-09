using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using BoundField = System.Web.UI.WebControls.BoundField;

namespace InSite.Cmds.Actions.Reports
{
    public partial class UnprioritizedCompetencies : AdminBasePage, ICmdsUserControl
    {
        private class CompetencyItem
        {
            public string Number { get; set; }
            public string Summary { get; set; }
            public string Profile { get; set; }
            public string TimeSensitive { get; set; }
            public string Lifetime { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Department.AutoPostBack = true;
            Department.ValueChanged += Department_ValueChanged;

            CreateReportButton.Click += CreateReportButton_Click;
            DownloadXlsx.Click += DownloadXlsx_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);
                InitSelectorsByCompany();
            }
        }

        private void Department_ValueChanged(object sender, EventArgs e)
        {
            CurrentProfile.Filter.DepartmentIdentifier = Department.Value;
            CurrentProfile.Value = null;
        }

        private void CreateReportButton_Click(object sender, EventArgs e)
        {
            InitData();

            PreviewSection.Visible = true;
            PreviewSection.IsSelected = true;
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            ExportSearchResultsToXlsx();
        }

        private void InitData()
        {
            var groups = CreateDataSource();

            var gv = new GridView()
            {
                AutoGenerateColumns = false,
                CssClass = "table table-striped"
            };
            gv.Columns.Add(new BoundField() { HeaderText = @"Competency", DataField = "Number" });
            gv.Columns.Add(new BoundField() { HeaderText = @"Competency Summary", DataField = "Summary" });
            gv.Columns.Add(new BoundField() { HeaderText = @"Profile", DataField = "Profile" });
            gv.Columns.Add(new BoundField() { HeaderText = @"Time-Sensitive", DataField = "TimeSensitive", HeaderStyle = { Wrap = false } });

            gv.DataSource = groups;
            gv.DataBind();

            place.Controls.Add(gv);

            PreviewSection.Visible = true;
            PreviewSection.IsSelected = true;
        }

        private void InitSelectorsByCompany()
        {
            Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            Department.Value = null;

            CurrentProfile.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            CurrentProfile.Filter.DepartmentIdentifier = null;
            CurrentProfile.Value = null;
        }

        private IEnumerable<CompetencyItem> CreateDataSource()
        {
            var data = CmdsReportHelper.SelectDepartmentCompetencies(
                Department.Value.Value,
                CurrentProfile.Value,
                string.IsNullOrEmpty(TimeSensitive.SelectedValue) ? (bool?)null : TimeSensitive.SelectedValue == "1",
                null,
                true,
                false
            );

            return data
                .OrderBy(y => y.Number).ThenBy(y => y.Summary).ThenBy(y => y.ProfileNumber)
                .Select(y => new CompetencyItem
                {
                    Number = y.Number,
                    Summary = y.Summary,
                    Profile = (!string.IsNullOrEmpty(y.ProfileNumber) ? y.ProfileNumber + ": " : "") + y.ProfileTitle,
                    TimeSensitive = y.IsTimeSensitive ? "Yes" : "No",
                    Lifetime = y.ValidForText
                })
                .ToList();
        }

        private void ExportSearchResultsToXlsx()
        {
            const string name = "Unprioritized Competencies";
            var helper = new XlsxExportHelper();
            helper.Map("Number", "Number", null, 20, HorizontalAlignment.Left);
            helper.Map("Summary", "Summary", null, 50, HorizontalAlignment.Left);
            helper.Map("Profile", "Profile", null, 40, HorizontalAlignment.Left);
            helper.Map("TimeSensitive", "Time-Sensitive", null, 20, HorizontalAlignment.Left);
            ReportXlsxHelper.ExportToXlsx(name, helper.GetXlsxBytes(CreateDataSource(), name));
        }
    }
}
