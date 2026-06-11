using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Cmds.Actions.Reporting.Report
{
    using ReportDataSource = GroupTable<TrainingExpiryDates.CompanyGroupNode, DefaultGroupNode<DefaultGroupLeaf>, TrainingExpiryDates.CellData>;

    public partial class TrainingExpiryDates : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const string CloseUrl = "/ui/admin/reporting";
        private const string StatusValid = "Valid";
        private const string ColorExpired = "#ff6347";
        private const string ColorExpiringSoon = "#ffff99";

        #endregion

        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid[] Departments { get; set; }
            public Guid[] Achievements { get; set; }
            public Guid[] Learners { get; set; }
            public bool? IsRequired { get; set; }
            public string[] MembershipFunctions { get; set; }
            public string CredentialStatus { get; set; }
            public DateTime? CompletedSince { get; set; }
            public DateTime? CompletedBefore { get; set; }
            public bool ExcludeSelfDeclared { get; set; }
        }

        internal class CompanyGroupNode : GroupNode<DefaultGroupLeaf>, IComparable<CompanyGroupNode>
        {
            #region Properties

            public string Name { get; set; }

            public IReadOnlyList<string> Departments => _departmentsList;

            #endregion

            #region Fields

            private readonly List<string> _departmentsList = new List<string>();
            private readonly HashSet<Guid> _departmentsHash = new HashSet<Guid>();

            #endregion

            #region Methods

            public bool TryAddDepartment(Guid id, string name)
            {
                if (_departmentsHash.Contains(id))
                    return false;

                _departmentsHash.Add(id);
                _departmentsList.AddSorted(name);

                return true;
            }

            public int CompareTo(CompanyGroupNode other) => other == null ? 1 : Name.CompareTo(other.Name);

            #endregion
        }

        internal class CellData
        {
            #region Properties

            public string Text { get; private set; }

            public string Color { get; private set; }

            #endregion

            #region Construction

            public CellData(CmdsReportHelper.TrainingExpiryDate row)
            {
                Text = GetText(row);
                Color = GetColor(row);
            }

            #endregion

            #region Methods

            private static string GetText(CmdsReportHelper.TrainingExpiryDate row)
            {
                if (row == null)
                    return string.Empty;

                string result;
                if (row.Status == StatusValid)
                {
                    if (row.ExpirationDate.HasValue)
                        result = $"{row.ExpirationDate.Value:MM'/'dd'/'yy}";
                    else if (row.DateCompleted.HasValue)
                        result = "Done";
                    else
                        result = string.Empty;
                }
                else if (row.ExpirationDate.HasValue)
                {
                    result = $"{row.ExpirationDate.Value:MM'/'dd'/'yy}";
                }
                else
                {
                    result = string.Empty;
                }

                if (row.IsRequired)
                    result += (result.Length > 0 ? " " : string.Empty) + "*";

                return result;
            }

            private static string GetColor(CmdsReportHelper.TrainingExpiryDate row)
            {
                if (row == null)
                    return null;

                if (row.Status != StatusValid)
                    return ColorExpired;

                if (!row.ExpirationDate.HasValue)
                    return null;

                var now = DateTimeOffset.Now;
                var expires = row.ExpirationDate.Value;
                if (expires >= now && expires <= now.AddMonths(3))
                    return ColorExpiringSoon;

                return null;
            }

            #endregion
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

            Criteria.MessageRaised += (type, message) => ScreenStatus.AddMessage(type, message);

            DownloadXlsx.Click += DownloadXlsx_Click;
            ReportButton.Click += ReportButton_Click;

            EmployeeRepeater.ItemCreated += EmployeeRepeater_ItemCreated;
            EmployeeRepeater.ItemDataBound += EmployeeRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CloseButton1.NavigateUrl = CloseUrl;
            CloseButton2.NavigateUrl = CloseUrl;
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                ReportTab.Visible = false;
                return;
            }

            LoadReport();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            if (CurrentParameters == null)
                return;

            var dataSource = GetReportDataSource();
            if (!dataSource.HasData)
                return;

            var xlsxSheet = new XlsxWorksheet(Route.Title);
            var (companyHeaderStyle, achievementHeaderStyle, dataCellStyle) = BuildXlsxStyles();

            WriteXlsxHeaders(xlsxSheet, dataSource, companyHeaderStyle, achievementHeaderStyle);
            WriteXlsxRows(xlsxSheet, dataSource, dataCellStyle);

            ReportXlsxHelper.Export(xlsxSheet);
        }

        private void EmployeeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var isContent = IsContentItem(e);
            if (!isContent)
                return;

            var employeeGroup = (DefaultGroupNode<DefaultGroupLeaf>)e.Item.DataItem;
            var departmentRepeater = (Repeater)e.Item.FindControl("DepartmentRepeater");
            departmentRepeater.DataSource = employeeGroup.Children;
            departmentRepeater.DataBind();
        }

        private void EmployeeRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            var isContent = IsContentItem(e);
            if (!isContent)
                return;

            var departmentRepeater = (Repeater)e.Item.FindControl("DepartmentRepeater");
            departmentRepeater.ItemDataBound += DepartmentRepeater_ItemDataBound;
        }

        private void DepartmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var isContent = IsContentItem(e);
            if (!isContent)
                return;

            var departmentGroup = (DefaultGroupLeaf)e.Item.DataItem;
            var table = (ReportDataSource)departmentGroup.Root;
            var cellRepeater = (Repeater)e.Item.FindControl("CellRepeater");
            cellRepeater.DataSource = table.Columns
                .SelectMany(x => x.Children)
                .Select(x => table.GetCell(x, departmentGroup))
                .ToArray();
            cellRepeater.DataBind();
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            ReportTab.Visible = false;

            CurrentParameters = new SearchParameters
            {
                Departments = Criteria.DepartmentValues,
                Achievements = Criteria.SelectedAchievements,
                Learners = Criteria.LearnerValues,
                IsRequired = Criteria.IsRequiredFilter,
                MembershipFunctions = Criteria.MembershipFunctions,
                CredentialStatus = Criteria.CredentialStatusFilter,
                CompletedSince = Criteria.CompletedSinceFilter,
                CompletedBefore = Criteria.CompletedBeforeFilter,
                ExcludeSelfDeclared = Criteria.ExcludeSelfDeclared
            };

            if (!Criteria.ValidateNarrowSelection(out var error))
            {
                ScreenStatus.AddMessage(AlertType.Error, error);
                return;
            }

            ReportDataSource dataSource;

            try
            {
                dataSource = GetReportDataSource();
            }
            catch (EntityCommandExecutionException ecex)
            {
                if (ecex.InnerException is SqlException sqex && sqex.Number == -2)
                {
                    ScreenStatus.AddMessage(
                        AlertType.Error,
                        "The report generation took too long to complete. Please try selecting fewer items.");
                    return;
                }

                throw;
            }

            if (!dataSource.HasData)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            CompanyHeaderRepeater.DataSource = dataSource.Columns;
            CompanyHeaderRepeater.DataBind();

            AchievementHeaderRepeater.DataSource = dataSource.Columns.SelectMany(x => x.Children);
            AchievementHeaderRepeater.DataBind();

            EmployeeRepeater.DataSource = dataSource.Rows;
            EmployeeRepeater.DataBind();
        }

        private ReportDataSource GetReportDataSource()
        {
            var organizations = GetOrganizationIds();

            var rows = CmdsReportHelper.SelectTrainingExpiryDates(
                organizations,
                CurrentParameters.Departments,
                CurrentParameters.Achievements,
                CurrentParameters.Learners,
                CurrentParameters.IsRequired,
                achievementType: null,
                membershipFunctions: CurrentParameters.MembershipFunctions,
                credentialStatus: CurrentParameters.CredentialStatus,
                completedSince: CurrentParameters.CompletedSince,
                completedBefore: CurrentParameters.CompletedBefore,
                excludeSelfDeclared: CurrentParameters.ExcludeSelfDeclared);

            var result = new ReportDataSource();

            foreach (var row in rows)
            {
                // Column

                var companyGroup = result.Columns.GetOrAdd(
                    () => new CompanyGroupNode { Name = row.CompanyName },
                    row.OrganizationIdentifier);
                var achievementGroup = companyGroup.Children.GetOrAdd(
                    () => new DefaultGroupLeaf { Text = row.AchievementTitle },
                    row.AchievementIdentifier);

                companyGroup.TryAddDepartment(row.DepartmentIdentifier, row.DepartmentName);

                // Row

                var employeeGroup = result.Rows.GetOrAdd(
                    () => new DefaultGroupNode<DefaultGroupLeaf> { Text = row.FullName },
                    row.UserIdentifier);
                var departmentGroup = employeeGroup.Children.GetOrAdd(
                    () => new DefaultGroupLeaf { Text = row.DepartmentName },
                    row.DepartmentIdentifier);

                // Cell

                result.AddCell(achievementGroup, departmentGroup, () => new CellData(row));
            }

            return result;
        }

        #endregion

        #region Xlsx export

        private static (XlsxCellStyle CompanyHeader, XlsxCellStyle AchievementHeader, XlsxCellStyle DataCell) BuildXlsxStyles()
        {
            var companyHeader = new XlsxCellStyle
            {
                BackgroundColor = Color.FromArgb(105, 105, 105),
                FontColor = Color.White,
                IsBold = true,
            };
            var achievementHeader = new XlsxCellStyle
            {
                BackgroundColor = Color.FromArgb(61, 120, 216),
                FontColor = Color.White,
                Align = HorizontalAlignment.Center,
                VAlign = XlsxCellVAlign.Center,
                WrapText = true,
                IsBold = true,
            };
            var dataCell = new XlsxCellStyle
            {
                WrapText = false,
                Align = HorizontalAlignment.Center,
            };
            return (companyHeader, achievementHeader, dataCell);
        }

        private static void WriteXlsxHeaders(
            XlsxWorksheet sheet,
            ReportDataSource dataSource,
            XlsxCellStyle companyHeaderStyle,
            XlsxCellStyle achievementHeaderStyle)
        {
            const int CompanyHeaderRow = 0;
            const int AchievementHeaderRow = 1;

            sheet.Columns[0].Width = 25;
            sheet.Columns[1].Width = 20;

            var companyColIndex = 0;
            sheet.Cells.Add(new XlsxCell(companyColIndex++, CompanyHeaderRow) { Style = companyHeaderStyle });
            sheet.Cells.Add(new XlsxCell(companyColIndex++, CompanyHeaderRow) { Style = companyHeaderStyle });

            var achievementColIndex = 0;
            sheet.Cells.Add(new XlsxCell(achievementColIndex++, AchievementHeaderRow) { Style = achievementHeaderStyle, Value = "Employee" });
            sheet.Cells.Add(new XlsxCell(achievementColIndex++, AchievementHeaderRow) { Style = achievementHeaderStyle, Value = "Department" });

            foreach (var companyGroup in dataSource.Columns)
            {
                sheet.Cells.Add(new XlsxCell(companyColIndex, CompanyHeaderRow, companyGroup.Children.Count)
                {
                    Style = companyHeaderStyle,
                    Value = $"Worker Training Expiry Dates for {companyGroup.Name} :: {string.Join(", ", companyGroup.Departments)}",
                });

                companyColIndex += companyGroup.Children.Count;

                foreach (var achievement in companyGroup.Children)
                {
                    sheet.Columns[achievementColIndex].Width = 13;
                    sheet.Cells.Add(new XlsxCell(achievementColIndex, AchievementHeaderRow)
                    {
                        Style = achievementHeaderStyle,
                        Value = achievement.Text,
                    });

                    achievementColIndex++;
                }
            }
        }

        private static void WriteXlsxRows(XlsxWorksheet sheet, ReportDataSource dataSource, XlsxCellStyle dataCellStyle)
        {
            const int FirstDataRow = 2;
            const int EmployeeColumn = 0;
            const int DepartmentColumn = 1;

            var rowIndex = FirstDataRow;
            var columnLeaves = dataSource.Columns.SelectMany(x => x.Children).ToArray();

            foreach (var employeeGroup in dataSource.Rows)
            {
                sheet.Cells.Add(new XlsxCell(EmployeeColumn, rowIndex, rowSpan: employeeGroup.Children.Count) { Value = employeeGroup.Text });

                foreach (var departmentLeaf in employeeGroup.Children)
                {
                    var colIndex = DepartmentColumn;

                    sheet.Cells.Add(new XlsxCell(colIndex++, rowIndex) { Value = departmentLeaf.Text });

                    foreach (var columnLeaf in columnLeaves)
                    {
                        var cellData = dataSource.GetCell(columnLeaf, departmentLeaf);

                        if (cellData != null)
                        {
                            var style = dataCellStyle.Copy();

                            style.BackgroundColor = cellData.Color != null
                                ? ColorTranslator.FromHtml(cellData.Color)
                                : Color.Transparent;

                            sheet.Cells.Add(new XlsxCell(colIndex, rowIndex)
                            {
                                Style = style,
                                Value = cellData.Text
                            });
                        }

                        colIndex++;
                    }

                    rowIndex++;
                }
            }
        }

        #endregion

        #region Helper methods

        private Guid[] GetOrganizationIds()
        {
            var organizations = new List<Guid> { Organization.Identifier };

            if (ServiceLocator.Partition.IsE03())
                organizations.Add(ServiceLocator.AppSettings.Application.Organizations.Global);

            return organizations.ToArray();
        }

        #endregion
    }
}
