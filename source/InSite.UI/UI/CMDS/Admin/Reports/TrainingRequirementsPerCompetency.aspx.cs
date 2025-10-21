using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class TrainingRequirementsPerCompetency : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid? OrganizationIdentifier { get; set; }
            public Guid[] Departments { get; set; }
            public string Status { get; set; }
        }

        private class DepartmentCompetencyList
        {
            public Guid OrganizationIdentifier { get; set; }
            public string CompanyName { get; set; }

            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }

            public IEnumerable<CompetencyInfo> Competencies { get; set; }
        }

        private class CompetencyInfo
        {
            public string Number { get; set; }
            public string Summary { get; set; }

            public IEnumerable<EmployeeInfo> Employees { get; set; }
        }

        private class EmployeeInfo
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
        }

        [Serializable]
        private class DepartmentInfo
        {
            public Guid Identifier { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
        }

        #endregion

        #region Properties

        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;
        private PersonFinderSecurityInfoWrapper FinderSecurityInfo => _finderSecurityInfo
            ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        private SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        protected Guid? CurrentDepartmentIdentifier
        {
            get => (Guid)ViewState[nameof(CurrentDepartmentIdentifier)];
            set => ViewState[nameof(CurrentDepartmentIdentifier)] = value;
        }

        private IEnumerable<CmdsReportHelper.TrainingRequirementPerCompetency> Data
        {
            get => (IEnumerable<CmdsReportHelper.TrainingRequirementPerCompetency>)ViewState[nameof(Data)];
            set => ViewState[nameof(Data)] = value;
        }

        private List<DepartmentInfo> Departments
        {
            get => (List<DepartmentInfo>)ViewState[nameof(Departments)];
            set => ViewState[nameof(Departments)] = value;
        }

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadXlsx.Click += DownloadXlsx_Click;

            SearchButton.Click += SearchButton_Click;

            CompetencyRepeater.ItemDataBound += CompetencyRepeater_ItemDataBound;

            DepartmentRepeater.ItemCommand += DepartmentRepeater_ItemCommand;
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

            var hasDepartments = DepartmentIdentifier.GetCount() > 0;

            DepartmentIdentifier.EmptyMessage = hasDepartments ? "All Departments" : "None";
            SearchButton.Enabled = hasDepartments;

            FinderSecurityInfo.LoadPermissions();
        }

        #endregion

        #region Event handlers

        private void CompetencyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var competencyInfo = (CompetencyInfo)e.Item.DataItem;

            var competencyRepeater = (Repeater)e.Item.FindControl("EmployeeRepeater");
            competencyRepeater.DataSource = competencyInfo.Employees;
            competencyRepeater.DataBind();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            const string reportName = "Training Requirements per Competency";

            if (CurrentParameters == null)
                return;

            var dataSource = GetReportDataSource(CurrentParameters.Departments);
            if (!dataSource.Any())
                return;

            using (var excel = new ExcelPackage())
            {
                excel.Workbook.CalcMode = ExcelCalcMode.Manual;

                var cellXfs = excel.Workbook.Styles.CellXfs[0];
                cellXfs.Font.Name = "Arial";
                cellXfs.Font.Size = 10;
                cellXfs.VerticalAlignment = ExcelVerticalAlignment.Top;
                cellXfs.WrapText = true;

                var cellStyleXfs = excel.Workbook.Styles.CellStyleXfs[0];
                cellStyleXfs.Font.Name = "Arial";
                cellStyleXfs.Font.Size = 10;
                cellStyleXfs.VerticalAlignment = ExcelVerticalAlignment.Top;

                var reportTitleStyle = excel.Workbook.Styles.CreateNamedStyle("Report Title");
                reportTitleStyle.Style.Font.Bold = true;
                reportTitleStyle.Style.Font.Size = 14;
                reportTitleStyle.Style.Border.Bottom.Set(ExcelBorderStyle.Thin, Color.Black);

                var tableHeaderStyle = excel.Workbook.Styles.CreateNamedStyle("Table Header");
                tableHeaderStyle.Style.Font.Bold = true;
                tableHeaderStyle.Style.Border.Bottom.Set(ExcelBorderStyle.Thin, Color.Black);

                var sheet = excel.Workbook.Worksheets.Add(reportName);

                sheet.Column(1).Width = 11;
                sheet.Column(2).Width = 65;
                sheet.Column(3).Width = 20;

                var rowNumber = 1;

                var headerCell = sheet.Cells[rowNumber, 1, rowNumber, 3];
                headerCell.Merge = true;
                headerCell.Value = "Training Requirements per Competency";
                headerCell.StyleName = reportTitleStyle.Name;

                sheet.Row(rowNumber++).Height = 22.5;
                sheet.Row(rowNumber++).Height = 10;

                foreach (var department in dataSource)
                {
                    var companyLabelCell = sheet.Cells[rowNumber, 1];
                    companyLabelCell.Value = "Organization:";

                    var companyDataCell = sheet.Cells[rowNumber, 2, rowNumber, 3];
                    companyDataCell.Merge = true;
                    companyDataCell.Value = department.CompanyName;

                    rowNumber++;

                    var departmentLabelCell = sheet.Cells[rowNumber, 1];
                    departmentLabelCell.Value = "Department:";

                    var departmentDataCell = sheet.Cells[rowNumber, 2, rowNumber, 3];
                    departmentDataCell.Merge = true;
                    departmentDataCell.Value = department.DepartmentName;

                    rowNumber++;

                    var statusLabelCell = sheet.Cells[rowNumber, 1];
                    statusLabelCell.Value = "Status:";

                    var statusDataCell = sheet.Cells[rowNumber, 2, rowNumber, 3];
                    statusDataCell.Merge = true;
                    statusDataCell.Value = CurrentParameters.Status;

                    rowNumber++;

                    sheet.Row(rowNumber++).Height = 10;

                    var competencyHeaderCell = sheet.Cells[rowNumber, 1, rowNumber, 2];
                    competencyHeaderCell.Merge = true;
                    competencyHeaderCell.Value = "Competency";
                    competencyHeaderCell.StyleName = tableHeaderStyle.Name;

                    var workerHeaderCell = sheet.Cells[rowNumber, 3];
                    workerHeaderCell.Value = "Worker";
                    workerHeaderCell.StyleName = tableHeaderStyle.Name;

                    rowNumber++;

                    foreach (var competency in department.Competencies)
                    {
                        var competencyNumberCell = sheet.Cells[rowNumber, 1];
                        competencyNumberCell.Value = competency.Number;
                        competencyNumberCell.Style.Border.Right.Set(ExcelBorderStyle.Thin, Color.Silver);
                        competencyNumberCell.Style.Border.Bottom.Set(ExcelBorderStyle.Thin, Color.Silver);
                        competencyNumberCell.Style.Border.Left.Set(ExcelBorderStyle.Thin, Color.Silver);

                        var competencySummaryCell = sheet.Cells[rowNumber, 2];
                        competencySummaryCell.Value = competency.Summary;
                        competencySummaryCell.Style.WrapText = true;
                        competencySummaryCell.Style.Border.Right.Set(ExcelBorderStyle.Thin, Color.Silver);
                        competencySummaryCell.Style.Border.Bottom.Set(ExcelBorderStyle.Thin, Color.Silver);
                        competencySummaryCell.Style.Border.Left.Set(ExcelBorderStyle.Thin, Color.Silver);

                        foreach (var employee in competency.Employees)
                        {
                            var employeeCell = sheet.Cells[rowNumber, 3];
                            employeeCell.Value = employee.FullName;
                            employeeCell.Style.WrapText = true;
                            employeeCell.Style.Border.Right.Set(ExcelBorderStyle.Thin, Color.Silver);
                            employeeCell.Style.Border.Bottom.Set(ExcelBorderStyle.Thin, Color.Silver);
                            employeeCell.Style.Border.Left.Set(ExcelBorderStyle.Thin, Color.Silver);

                            rowNumber++;
                        }

                        sheet.Cells[rowNumber - 1, 1, rowNumber - 1, 3].Style.Border.Bottom.Set(ExcelBorderStyle.Thin, Color.Black);
                    }

                    sheet.Row(rowNumber++).Height = 20;
                }

                excel.Workbook.Properties.Title = reportName;

                excel.Workbook.Properties.Company = Organization.Name;
                excel.Workbook.Properties.Author = User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                if (rowNumber > 1)
                    sheet.PrinterSettings.PrintArea = sheet.Cells[1, 1, rowNumber - 1, 3];

                sheet.PrinterSettings.Orientation = eOrientation.Portrait;
                sheet.PrinterSettings.PaperSize = ePaperSize.A4;
                sheet.PrinterSettings.FitToPage = true;
                sheet.PrinterSettings.FitToWidth = 1;
                sheet.PrinterSettings.FitToHeight = 0;

                excel.Workbook.CalcMode = ExcelCalcMode.Automatic;

                ReportXlsxHelper.Export(reportName, excel);
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (!Page.IsValid)
                return;

            if (!LoadReport())
                ScreenStatus.AddMessage(AlertType.Information, "There is no data matching your criteria.");
        }

        private void DepartmentRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Show")
            {
                var department = Guid.Parse(e.CommandArgument.ToString());

                LoadDepartmentCompetencies(department);

                Page.MaintainScrollPositionOnPostBack = false;
            }
        }

        #endregion

        #region Data binding

        private bool LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                Departments = DepartmentIdentifier.Values,
                Status = StatusSelector.SelectedValue
            };

            if (CurrentParameters.Departments.Length == 0)
                CurrentParameters.Departments = DepartmentIdentifier.GetDataItems().Select(x => x.Value).ToArray();

            if (CurrentParameters.Departments.Length == 0)
                return false;

            Data = CmdsReportHelper.SelectTrainingRequirementsPerCompetency(CurrentParameters.Departments, CurrentParameters.Status);

            if (!Data.Any())
                return false;

            Departments = new List<DepartmentInfo>();

            DepartmentInfo currentDepartment = null;
            string prevCompetencyNumber = null;

            foreach (var dataItem in Data)
            {
                if (currentDepartment == null || currentDepartment.Identifier != dataItem.DepartmentIdentifier)
                {
                    currentDepartment = Departments.FirstOrDefault(x => x.Identifier == dataItem.DepartmentIdentifier);

                    if (currentDepartment == null)
                        Departments.Add(currentDepartment = new DepartmentInfo { Identifier = dataItem.DepartmentIdentifier, Name = dataItem.DepartmentName });
                }

                if (prevCompetencyNumber != dataItem.Number)
                {
                    currentDepartment.Count++;
                    prevCompetencyNumber = dataItem.Number;
                }
            }

            LoadDepartmentCompetencies(Departments[0].Identifier);

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DownloadCommandsPanel.Visible = Data.Any();

            return true;
        }

        private void LoadDepartmentCompetencies(Guid department)
        {
            CurrentDepartmentIdentifier = department;

            DepartmentRepeater.DataSource = Departments;
            DepartmentRepeater.DataBind();

            var dataSource = GetReportDataSource(new Guid[] { department }).FirstOrDefault()?.Competencies;

            CompetencyRepeater.DataSource = dataSource;
            CompetencyRepeater.DataBind();
        }

        private IEnumerable<DepartmentCompetencyList> GetReportDataSource(Guid[] departments)
        {
            var organization = OrganizationSearch.Select(CurrentParameters.OrganizationIdentifier.Value);

            return Data
                .Where(x => departments.Contains(x.DepartmentIdentifier))
                .GroupBy(x => x.DepartmentIdentifier)
                .Select(deptGroup =>
                {
                    var firstDept = deptGroup.First();

                    return new DepartmentCompetencyList
                    {
                        OrganizationIdentifier = organization.OrganizationIdentifier,
                        CompanyName = organization.CompanyName,
                        DepartmentIdentifier = firstDept.DepartmentIdentifier,
                        DepartmentName = firstDept.DepartmentName,

                        Competencies = deptGroup
                            .GroupBy(x => x.Number)
                            .Select(competencyGroup =>
                            {
                                var firstCompetency = competencyGroup.First();

                                return new CompetencyInfo
                                {
                                    Number = firstCompetency.Number,
                                    Summary = firstCompetency.Summary,

                                    Employees = competencyGroup
                                        .Select(x => new EmployeeInfo
                                        {
                                            UserIdentifier = x.UserIdentifier,
                                            FullName = x.FullName
                                        })
                                        .OrderBy(x => x.FullName)
                                        .ToArray()
                                };
                            })
                            .Where(x => x.Employees.Any())
                            .OrderBy(x => x.Number)
                            .ToArray()
                    };
                })
                .Where(x => x.Competencies.Any())
                .OrderBy(x => x.DepartmentName)
                .ToArray();
        }

        #endregion
    }
}