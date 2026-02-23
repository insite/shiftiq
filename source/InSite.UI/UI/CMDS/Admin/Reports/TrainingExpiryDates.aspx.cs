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
        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid[] Departments { get; set; }
            public Guid[] Achievements { get; set; }
            public Guid[] Learners { get; set; }
            public bool? IsRequired { get; set; }
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

            public int CompareTo(CompanyGroupNode other) => Name.CompareTo(other.Name);

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
                var result = string.Empty;

                if (row != null)
                {
                    if (row.Status == "Valid")
                    {
                        if (row.ExpirationDate.HasValue)
                            result = $"{row.ExpirationDate.Value:MM'/'dd'/'yy}";
                        else if (row.DateCompleted.HasValue)
                            result = "Done";
                    }
                    else if (row.ExpirationDate.HasValue)
                    {
                        result = $"{row.ExpirationDate.Value:MM'/'dd'/'yy}";
                    }

                    if (row.IsRequired)
                        result += (result.Length > 0 ? " " : string.Empty) + "*";
                }

                return result;
            }

            private static string GetColor(CmdsReportHelper.TrainingExpiryDate row)
            {
                string result = null;

                if (row != null)
                {
                    if (row.Status != "Valid")
                        result = "#ff6347"; // Red
                    else if (row.ExpirationDate.HasValue && row.ExpirationDate.Value >= DateTimeOffset.Now && row.ExpirationDate.Value <= DateTimeOffset.Now.AddMonths(3))
                        result = "#ffff99"; // Yellow
                }

                return result;
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

            FindDepartment.AutoPostBack = true;
            FindDepartment.ValueChanged += (s, a) => OnDepartmentChanged();

            FindProgram.AutoPostBack = true;
            FindProgram.ValueChanged += (s, a) => SetupFindAchievement();

            IsRequired.AutoPostBack = true;
            IsRequired.SelectedIndexChanged += (s, a) => SetupFindAchievement();

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

            FindDepartment.Filter.OrganizationIdentifier = Organization.Identifier;

            if (!Identity.HasAccessToAllCompanies)
                FindDepartment.Filter.UserIdentifier = User.UserIdentifier;

            OnDepartmentChanged();

            var closeUrl = "/ui/admin/reporting";
            CloseButton1.NavigateUrl = closeUrl;
            CloseButton2.NavigateUrl = closeUrl;
        }

        #endregion

        #region Event handlers

        private void OnDepartmentChanged()
        {
            FindLearner.Enabled = FindDepartment.HasValue;
            FindLearner.Filter.OrganizationIdentifier = Organization.Identifier;
            FindLearner.Filter.GroupDepartmentIdentifiers = FindDepartment.Values;
            FindLearner.Value = null;

            SetupFindAchievement();
        }

        private void SetupFindAchievement()
        {
            FindAchievement.Enabled = FindDepartment.HasValue;
            FindAchievement.Filter.DepartmentIdentifiers = FindDepartment.Values;
            FindAchievement.Filter.ProgramIdentifiers = FindProgram.Values;
            FindAchievement.Filter.HasMandatoryCredential = GetIsRequired();
            FindAchievement.Value = null;
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
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
            var headerStyle1 = new XlsxCellStyle
            {
                BackgroundColor = Color.FromArgb(105, 105, 105),
                FontColor = Color.White,
                IsBold = true,
            };
            var headerStyle2 = new XlsxCellStyle
            {
                BackgroundColor = Color.FromArgb(61, 120, 216),
                FontColor = Color.White,
                Align = HorizontalAlignment.Center,
                VAlign = XlsxCellVAlign.Center,
                WrapText = true,
                IsBold = true,
            };
            var dataCellStyle = new XlsxCellStyle
            {
                WrapText = false,
                Align = HorizontalAlignment.Center,
            };

            var companyColIndex = 0;
            var AchievementColIndex = 0;

            xlsxSheet.Columns[0].Width = 25;
            xlsxSheet.Columns[1].Width = 20;

            xlsxSheet.Cells.Add(new XlsxCell(companyColIndex++, 0) { Style = headerStyle1 });
            xlsxSheet.Cells.Add(new XlsxCell(companyColIndex++, 0) { Style = headerStyle1 });

            xlsxSheet.Cells.Add(new XlsxCell(AchievementColIndex++, 1) { Style = headerStyle2, Value = "Employee" });
            xlsxSheet.Cells.Add(new XlsxCell(AchievementColIndex++, 1) { Style = headerStyle2, Value = "Department" });

            foreach (var companyGroup in dataSource.Columns)
            {
                xlsxSheet.Cells.Add(new XlsxCell(companyColIndex, 0, companyGroup.Children.Count)
                {
                    Style = headerStyle1,
                    Value = $"Worker Training Expiry Dates for {companyGroup.Name} :: {string.Join(", ", companyGroup.Departments)}",
                });

                companyColIndex += companyGroup.Children.Count;

                foreach (var achievement in companyGroup.Children)
                {
                    xlsxSheet.Columns[AchievementColIndex].Width = 13;
                    xlsxSheet.Cells.Add(new XlsxCell(AchievementColIndex, 1)
                    {
                        Style = headerStyle2,
                        Value = achievement.Text,
                    });

                    AchievementColIndex++;
                }
            }

            var rowIndex = 2;
            var columnLeaves = dataSource.Columns.SelectMany(x => x.Children).ToArray();

            foreach (var employeeGroup in dataSource.Rows)
            {
                xlsxSheet.Cells.Add(new XlsxCell(0, rowIndex, rowSpan: employeeGroup.Children.Count) { Value = employeeGroup.Text });

                foreach (var departmentLeaf in employeeGroup.Children)
                {
                    var colIndex = 1;

                    xlsxSheet.Cells.Add(new XlsxCell(colIndex++, rowIndex) { Value = departmentLeaf.Text });

                    foreach (var columnLeaf in columnLeaves)
                    {
                        var cellData = dataSource.GetCell(columnLeaf, departmentLeaf);

                        if (cellData != null)
                        {
                            var style = dataCellStyle.Copy();

                            style.BackgroundColor = cellData.Color != null
                                ? ColorTranslator.FromHtml(cellData.Color)
                                : Color.Transparent;

                            xlsxSheet.Cells.Add(new XlsxCell(colIndex, rowIndex)
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

            ReportXlsxHelper.Export(xlsxSheet);
        }

        private void EmployeeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var employeeGroup = (DefaultGroupNode<DefaultGroupLeaf>)e.Item.DataItem;
            var departmentRepeater = (Repeater)e.Item.FindControl("DepartmentRepeater");
            departmentRepeater.DataSource = employeeGroup.Children;
            departmentRepeater.DataBind();
        }

        private void EmployeeRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var departmentRepeater = (Repeater)e.Item.FindControl("DepartmentRepeater");
            departmentRepeater.ItemDataBound += DepartmentRepeater_ItemDataBound;
        }

        private void DepartmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var departmentGroup = (DefaultGroupLeaf)e.Item.DataItem;
            var table = (ReportDataSource)departmentGroup.Root;
            var cellRepeater = (Repeater)e.Item.FindControl("CellRepeater");
            cellRepeater.DataSource = table.Columns.SelectMany(x => x.Children).Select(x => table.GetCell(x, departmentGroup));
            cellRepeater.DataBind();
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                Departments = FindDepartment.Values,
                Achievements = FindAchievement.Values,
                Learners = FindLearner.Values,
                IsRequired = GetIsRequired()
            };

            if (CurrentParameters.Departments.Length == 0 || CurrentParameters.Achievements.Length == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "The departments you have selected do not have any training achievements.");
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

                throw ecex;
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
            var rows = CmdsReportHelper.SelectTrainingExpiryDates(
                CurrentParameters.Departments,
                CurrentParameters.Achievements,
                CurrentParameters.Learners,
                CurrentParameters.IsRequired,
                Constants.DefaultPassingGrade);

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

        #region Helper methods

        private bool? GetIsRequired()
        {
            return IsRequired.SelectedIndex > 0 ? bool.Parse(IsRequired.SelectedValue) : (bool?)null;
        }

        #endregion
    }
}
