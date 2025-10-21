using System;
using System.Collections;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class UserTrainingDetail : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private static class Style
        {
            public static readonly XlsxCellStyle HeaderStyle = new XlsxCellStyle { IsBold = true };
            public static readonly XlsxCellStyle ColumnStyle = new XlsxCellStyle();

            public const string DateFormat = "MMM d, yyyy";
        }

        #endregion

        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid DepartmentIdentifier { get; set; }
            public Guid AchievementIdentifier { get; set; }
        }

        #endregion

        #region Properties

        private SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += DepartmentIdentifier_ValueChanged;

            AchievementLabel.AutoPostBack = true;
            AchievementLabel.SelectedIndexChanged += AchievementLabel_SelectedIndexChanged;

            DownloadXlsx.Click += (s, a) => ExportXlsx(CurrentParameters);

            ReportButton.Click += ReportButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            DepartmentIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!Identity.HasAccessToAllCompanies)
                DepartmentIdentifier.Filter.UserIdentifier = User.UserIdentifier;

            LoadAchievementTypes();

            AchievementIdentifier.Filter.AchievementType = "NA";
            AchievementIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            AchievementIdentifier.Filter.GlobalOrCompanySpecific = true;
            AchievementIdentifier.IncludeUserAchievement = true;
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void DepartmentIdentifier_ValueChanged(object sender, EventArgs e)
        {
            LoadAchievementTypes();

            AchievementIdentifier.Filter.DepartmentIdentifier = DepartmentIdentifier.Value ?? Guid.Empty;
            AchievementIdentifier.Filter.AchievementType = AchievementLabel.SelectedValue.IfNullOrEmpty("NA");
            AchievementIdentifier.Value = null;
        }

        private void AchievementLabel_SelectedIndexChanged(object sender, EventArgs e)
        {
            AchievementIdentifier.Filter.AchievementType = AchievementLabel.SelectedValue.IfNullOrEmpty("NA");
            AchievementIdentifier.Value = null;
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                DepartmentIdentifier = DepartmentIdentifier.Value.Value,
                AchievementIdentifier = AchievementIdentifier.Value.Value
            };

            var data = GetReportDataSource(CurrentParameters);

            if (data.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DataRepeater.DataSource = data;
            DataRepeater.DataBind();

            AchievementTitle.Text = VCmdsAchievementSearch.SelectFirst(x => x.AchievementIdentifier == AchievementIdentifier.Value.Value).AchievementTitle;
            CompanyName.Text = OrganizationSearch.Select(CurrentIdentityFactory.ActiveOrganizationIdentifier).Name;
            DepartmentName.Text = DepartmentIdentifier.Item.Text;
        }

        private IList GetReportDataSource(SearchParameters parameters)
        {
            return VCmdsCredentialSearch.SelectCredentials(parameters.DepartmentIdentifier, parameters.AchievementIdentifier)
                .Select(x => new
                {
                    FirstName = x.UserFirstName,
                    LastName = x.UserLastName,
                    FullName = x.UserFullName,
                    DateCompleted = x.CredentialGranted,
                    ExpirationDate = x.CredentialExpirationExpected ?? x.CredentialExpired,
                    Status = x.CredentialIsMandatory ? "Required" : "Optional"
                })
                .ToList();
        }

        #endregion

        #region Helper methods

        private void LoadAchievementTypes()
        {
            var oldSelectedValue = AchievementLabel.SelectedValue;

            AchievementLabel.Items.Clear();

            var labels = ServiceLocator.AchievementSearch.GetAchievementLabels(Organization.Identifier);
            var types = VCmdsAchievementSearch.SelectAchievementLabels(Organization.Code, labels, null);

            if (!DepartmentIdentifier.HasValue)
            {
                foreach (var subtype in types.Items)
                    AchievementLabel.Items.Add(new System.Web.UI.WebControls.ListItem(subtype.Text, subtype.Value));
            }
            else
            {
                var counts = VCmdsAchievementSearch.CountCredentialsByAchievementLabel(DepartmentIdentifier.Value.Value);

                foreach (var subtype in types.Items)
                {
                    var count = counts.FirstOrDefault(x => string.Equals(x.Type, subtype.Value, StringComparison.OrdinalIgnoreCase))?.Count ?? 0;
                    var item = new System.Web.UI.WebControls.ListItem($"{subtype.Text} ({count})", subtype.Value);

                    item.Enabled = count > 0;

                    AchievementLabel.Items.Add(item);
                }
            }

            if (oldSelectedValue.IsNotEmpty())
            {
                var selectedItem = AchievementLabel.Items.FindByValue(oldSelectedValue);

                if (selectedItem != null && selectedItem.Enabled)
                    AchievementLabel.SelectedValue = oldSelectedValue;
            }
        }

        #endregion

        #region Helper methods: export (XLSX)

        private void ExportXlsx(SearchParameters parameters)
        {
            var achievement = VCmdsAchievementSearch.Select(parameters.AchievementIdentifier);
            var department = DepartmentSearch.Select(parameters.DepartmentIdentifier, x => x.Organization);

            var dataSource = GetReportDataSource(parameters);

            var xlsxSheet = new XlsxWorksheet("Worker Traning Details");
            xlsxSheet.Columns[0].Width = 20;
            xlsxSheet.Columns[1].Width = 20;
            xlsxSheet.Columns[2].Width = 17;
            xlsxSheet.Columns[3].Width = 17;
            xlsxSheet.Columns[4].Width = 15;

            var rowNumber = 0;

            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Organization:", Style = Style.HeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = department.Organization.CompanyName, Style = Style.ColumnStyle });
            rowNumber++;
            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Department:", Style = Style.HeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = department.DepartmentName, Style = Style.ColumnStyle });
            rowNumber++;
            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Achievement:", Style = Style.HeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = achievement.AchievementTitle, Style = Style.ColumnStyle });
            rowNumber++;
            rowNumber++;

            xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "First Name", Style = Style.HeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = "Last Name", Style = Style.HeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = "Date Completed", Style = Style.HeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = "Expiration Date", Style = Style.HeaderStyle });
            xlsxSheet.Cells.Add(new XlsxCell(4, rowNumber) { Value = "Status", Style = Style.HeaderStyle });

            rowNumber++;

            foreach (var dataItem in dataSource)
            {
                var firstName = DataBinder.Eval(dataItem, "FirstName");
                var lastName = DataBinder.Eval(dataItem, "LastName");
                var dateCompleted = DataBinder.Eval(dataItem, "DateCompleted") as DateTimeOffset?;
                var expirationDate = DataBinder.Eval(dataItem, "ExpirationDate") as DateTimeOffset?;
                var status = DataBinder.Eval(dataItem, "Status");

                xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = firstName, Style = Style.ColumnStyle });
                xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = lastName, Style = Style.ColumnStyle });
                xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = dateCompleted?.UtcDateTime, Style = Style.ColumnStyle, Format = Style.DateFormat });
                xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = expirationDate?.UtcDateTime, Style = Style.ColumnStyle, Format = Style.DateFormat });
                xlsxSheet.Cells.Add(new XlsxCell(4, rowNumber) { Value = status, Style = Style.ColumnStyle });

                rowNumber++;
            }

            ReportXlsxHelper.Export(xlsxSheet);
        }

        #endregion
    }
}