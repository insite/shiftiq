using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Toolbox;

using Color = System.Drawing.Color;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class UserCompetencySummary : AdminBasePage
    {
        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid UserIdentifier { get; set; }
            public Guid? ProfileStandardIdentifier { get; set; }
            public bool IsPrimaryOnly { get; set; }
        }

        #endregion

        #region Constants

        private class Style
        {
            public static readonly XlsxCellStyle CompanyTitleStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left };
            public static readonly XlsxCellStyle AreasStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left, FontColor = Color.FromArgb(118, 118, 118) };
            public static readonly XlsxCellStyle ProfileTitleStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left, FontColor = Color.FromArgb(17, 109, 182) };
            public static readonly XlsxCellStyle LeftColumnStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            public static readonly XlsxCellStyle LeftColumnBoldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right };
            public static readonly XlsxCellStyle RightColumnStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left };
            public static readonly XlsxCellStyle RightColumnBoldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left };
        }

        #endregion

        #region Properties

        private SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Employee.AutoPostBack = true;
            Employee.ValueChanged += Employee_ValueChanged;

            ProfileMode.AutoPostBack = true;
            ProfileMode.SelectedIndexChanged += ProfileMode_SelectedIndexChanged;

            DownloadXlsx.Click += DownloadXlsx_Click;

            DataRepeater.ItemCreated += DataRepeater_ItemCreated;
            DataRepeater.ItemDataBound += DataRepeater_ItemDataBound;

            CreateReportButton.Click += CreateReportButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                Employee.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
                Employee.Filter.RoleType = new[] { MembershipType.Department, MembershipType.Organization };

                if (!Identity.HasAccessToAllCompanies)
                    Employee.Filter.DepartmentsForParentId = User.UserIdentifier;

                CurrentProfile.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
                CurrentProfile.Filter.ProfileUserIdentifier = Guid.Empty;
            }
        }

        #endregion

        #region Event handlers

        private void Employee_ValueChanged(object sender, EventArgs e)
        {
            CurrentProfile.Filter.ProfileUserIdentifier = Employee.Value ?? Guid.Empty;
            CurrentProfile.Value = null;
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            ExportSearchResultsToXlsx();
        }

        private void ProfileMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentProfile.Enabled = ProfileMode.SelectedValue == "Specific";
            CurrentProfile.Value = null;

            CurrentProfileRequired.Enabled = CurrentProfile.Enabled;
        }

        private void DataRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var profileRepeater = (Repeater)e.Item.FindControl("ProfileRepeater");
            profileRepeater.ItemDataBound += ProfileRepeater_ItemDataBound;
        }

        private void DataRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var dataItem = (CmdsReportHelper.UserCompetencySummary)e.Item.DataItem;

            var profileRepeater = (Repeater)e.Item.FindControl("ProfileRepeater");
            profileRepeater.DataSource = dataItem.Profiles;
            profileRepeater.DataBind();
        }

        private void ProfileRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var profile = (CmdsReportHelper.UserCompetencySummary.Profile)e.Item.DataItem;

            var managerRepeater = (Repeater)e.Item.FindControl("ManagerRepeater");
            managerRepeater.DataSource = profile.Managers;
            managerRepeater.DataBind();

            var statusRepeater = (Repeater)e.Item.FindControl("StatusRepeater");
            statusRepeater.DataSource = profile.Statuses;
            statusRepeater.DataBind();
        }

        private void CreateReportButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            CurrentParameters = new SearchParameters()
            {
                UserIdentifier = Employee.Value.Value,
                ProfileStandardIdentifier = CurrentProfile.Value,
                IsPrimaryOnly = ProfileMode.SelectedValue == "Primary"
            };

            var dataSource = CmdsReportHelper.SelectUserCompetencySummary(CurrentParameters.UserIdentifier, CurrentParameters.ProfileStandardIdentifier, CurrentParameters.IsPrimaryOnly);

            PreviewSection.Visible = true;
            PreviewSection.IsSelected = true;

            DownloadCommandsPanel.Visible = dataSource.Any();

            DataRepeater.DataSource = dataSource;
            DataRepeater.DataBind();

            PreviewSection.Visible = true;
            PreviewSection.IsSelected = true;
        }

        #endregion

        #region Helper methods: export (XLSX)

        private void ExportSearchResultsToXlsx()
        {
            var dataSource = CmdsReportHelper.SelectUserCompetencySummary(CurrentParameters.UserIdentifier, CurrentParameters.ProfileStandardIdentifier, CurrentParameters.IsPrimaryOnly);

            var xlsxSheet = new XlsxWorksheet("Worker Competency Summary");
            xlsxSheet.Columns[0].Width = 25;
            xlsxSheet.Columns[1].Width = 25;
            xlsxSheet.Columns[2].Width = 15;
            xlsxSheet.Columns[3].Width = 40;

            var rowNumber = 0;

            foreach (var dataItem in dataSource)
            {
                xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber++, 4) { Value = dataItem.CompanyName, Style = Style.CompanyTitleStyle });
                xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber++, 4) { Value = string.Join(", ", dataItem.Areas), Style = Style.AreasStyle });

                rowNumber++;

                foreach (var profile in dataItem.Profiles)
                {
                    xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber++, 4) { Value = profile.ProfileTitle, Style = Style.ProfileTitleStyle });

                    var profileDetailsRowNumber = AddProfileDetailsToXslx(xlsxSheet, rowNumber, profile);
                    var managersRowNumber = AddManagersToXslx(xlsxSheet, rowNumber, profile);

                    rowNumber = 1 + (profileDetailsRowNumber > managersRowNumber ? profileDetailsRowNumber : managersRowNumber);
                }
            }

            ReportXlsxHelper.Export(xlsxSheet);
        }

        private int AddProfileDetailsToXslx(XlsxWorksheet xlsxSheet, int rowNumber, CmdsReportHelper.UserCompetencySummary.Profile profile)
        {
            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Employee:", Style = Style.LeftColumnBoldStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = profile.FullName, Style = Style.RightColumnStyle });
            rowNumber++;

            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = (profile.IsPrimary ? "Primary" : "Secondary") + " Profile:", Style = Style.LeftColumnBoldStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = profile.ProfileTitle, Style = Style.RightColumnStyle });
            rowNumber++;

            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Validated Competencies:", Style = Style.LeftColumnBoldStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = profile.SumEmployeeCompetencies + " out of " + profile.SumTotalCompetencies + "  = " + profile.AvgEmployeePercent + "%", Style = Style.RightColumnStyle });
            rowNumber++;
            rowNumber++;

            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "DEVELOPMENT PLAN:", Style = Style.LeftColumnBoldStyle });
            rowNumber++;

            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Validation Status:", Style = Style.LeftColumnBoldStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = "# of Competencies", Style = Style.RightColumnBoldStyle });
            rowNumber++;

            foreach (var status in profile.Statuses)
            {
                xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = status.StatusName, Style = Style.LeftColumnStyle });
                xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = status.CompetencyCount, Style = Style.RightColumnStyle });
                rowNumber++;
            }

            return rowNumber;
        }

        private int AddManagersToXslx(XlsxWorksheet xlsxSheet, int rowNumber, CmdsReportHelper.UserCompetencySummary.Profile profile)
        {
            foreach (var manager in profile.Managers)
            {
                xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = "Validator:", Style = Style.LeftColumnBoldStyle });
                xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = manager.FullName, Style = Style.RightColumnBoldStyle });
                rowNumber++;

                xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = "Email:", Style = Style.LeftColumnStyle });
                xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = manager.EmailWork, Style = Style.RightColumnStyle });
                rowNumber++;

                xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = "Phone:", Style = Style.LeftColumnStyle });
                xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = manager.PhoneWork, Style = Style.RightColumnStyle });
                rowNumber++;
            }

            return rowNumber;
        }

        #endregion
    }
}
