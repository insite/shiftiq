using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class TrainingRequirementsPerUser : AdminBasePage, ICmdsUserControl
    {
        #region Fields

        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;

        #endregion

        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid? OrganizationIdentifier { get; set; }
            public string Departments { get; set; }
            public string Status { get; set; }
        }

        private class DepartmentInfo
        {
            public Guid OrganizationIdentifier { get; set; }
            public string CompanyName { get; set; }

            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }

            public IEnumerable<UserInfo> Users { get; set; }
        }

        private class UserInfo
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }

            public IEnumerable<CompetencyInfo> Competencies { get; set; }
        }

        private class CompetencyInfo
        {
            public string Number { get; set; }
            public string Summary { get; set; }
        }

        #endregion

        #region Properties

        private PersonFinderSecurityInfoWrapper FinderSecurityInfo => _finderSecurityInfo ??
            (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        private SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadXlsx.Click += DownloadXlsx_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            FinderSecurityInfo.LoadPermissions();

            InitSelectors();
        }

        #endregion

        #region Event handlers

        private void UserRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var userInfo = (UserInfo)e.Item.DataItem;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = userInfo.Competencies;
            competencyRepeater.DataBind();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            const string reportName = "Training Requirements per User";

            var parameters = new SearchParameters
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                Departments = GetDepartments(),
                Status = Status.SelectedValue
            };

            if (parameters.Departments.IsEmpty())
            {
                ScreenStatus.AddMessage(AlertType.Error, "At least one department must be selected.");
                return;
            }

            CurrentParameters = parameters;

            var dataSource = GetReportDataSource();
            if (!dataSource.Any())
                return;

            using (var excel = new ExcelPackage())
            {
                var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                defaultStyle.Font.Name = "Arial";
                defaultStyle.Font.Size = 10;
                defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;

                var reportTitleStyle = excel.Workbook.Styles.CreateNamedStyle("Report Title");
                reportTitleStyle.Style.Font.Bold = true;
                reportTitleStyle.Style.Font.Size = 14;
                reportTitleStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                reportTitleStyle.Style.Border.Bottom.Color.SetColor(Color.Black);

                var tableHeaderStyle = excel.Workbook.Styles.CreateNamedStyle("Table Header");
                tableHeaderStyle.Style.Font.Bold = true;
                tableHeaderStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                tableHeaderStyle.Style.Border.Bottom.Color.SetColor(Color.Black);

                var tableCellStyle = excel.Workbook.Styles.CreateNamedStyle("Table Cell");
                tableCellStyle.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                tableCellStyle.Style.Border.Top.Color.SetColor(Color.Silver);
                tableCellStyle.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                tableCellStyle.Style.Border.Right.Color.SetColor(Color.Silver);
                tableCellStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                tableCellStyle.Style.Border.Bottom.Color.SetColor(Color.Silver);
                tableCellStyle.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                tableCellStyle.Style.Border.Left.Color.SetColor(Color.Silver);

                var sheet = excel.Workbook.Worksheets.Add(reportName);

                sheet.Column(1).Width = 18;
                sheet.Column(2).Width = 12;
                sheet.Column(3).Width = 70;

                var rowNumber = 1;

                var headerCell = sheet.Cells[rowNumber, 1, rowNumber, 3];
                headerCell.Merge = true;
                headerCell.Value = "Training Requirements per User";
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

                    var userHeaderCell = sheet.Cells[rowNumber, 1];
                    userHeaderCell.Value = "Worker";
                    userHeaderCell.StyleName = tableHeaderStyle.Name;

                    var competencyHeaderCell = sheet.Cells[rowNumber, 2, rowNumber, 3];
                    competencyHeaderCell.Merge = true;
                    competencyHeaderCell.Value = "Competency";
                    competencyHeaderCell.StyleName = tableHeaderStyle.Name;

                    rowNumber++;

                    foreach (var user in department.Users)
                    {
                        var userCell = sheet.Cells[rowNumber, 1];
                        userCell.Value = user.FullName;
                        userCell.StyleName = tableCellStyle.Name;

                        foreach (var competency in user.Competencies)
                        {
                            var competencyNumberCell = sheet.Cells[rowNumber, 2];
                            competencyNumberCell.Value = competency.Number;
                            competencyNumberCell.StyleName = tableCellStyle.Name;

                            var competencySummaryCell = sheet.Cells[rowNumber, 3];
                            competencySummaryCell.Value = competency.Summary;
                            competencySummaryCell.StyleName = tableCellStyle.Name;
                            competencySummaryCell.Style.WrapText = true;

                            sheet.Row(rowNumber++).Style.WrapText = true;
                        }
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

                ReportXlsxHelper.Export(reportName, excel);
            }
        }

        #endregion

        #region Data binding

        private IEnumerable<DepartmentInfo> GetReportDataSource()
        {
            Server.ScriptTimeout = 6 * 60;

            return CmdsReportHelper.SelectTrainingRequirementsPerUser(CurrentParameters.OrganizationIdentifier.Value, CurrentParameters.Departments, CurrentParameters.Status)
                .Where(x => x.UserIdentifier.HasValue)
                .GroupBy(x => new MultiKey(x.OrganizationIdentifier, x.DepartmentIdentifier))
                .Select(deptGroup =>
                {
                    var firstDept = deptGroup.First();

                    return new DepartmentInfo
                    {
                        OrganizationIdentifier = firstDept.OrganizationIdentifier,
                        CompanyName = firstDept.CompanyName,
                        DepartmentIdentifier = firstDept.DepartmentIdentifier,
                        DepartmentName = firstDept.DepartmentName,

                        Users = deptGroup
                            .GroupBy(x => x.UserIdentifier)
                            .Select(userGroup =>
                            {
                                var firstUser = userGroup.First();

                                return new UserInfo
                                {
                                    UserIdentifier = firstUser.UserIdentifier.Value,
                                    FullName = firstUser.FullName,
                                    Competencies = userGroup.Select(x => new CompetencyInfo
                                    {
                                        Number = x.Number,
                                        Summary = x.Summary
                                    })
                                };
                            })
                            .Where(x => x.Competencies.Any())
                            .OrderBy(x => x.FullName)
                            .ToArray()
                    };
                })
                .Where(x => x.Users.Any())
                .OrderBy(x => x.DepartmentName)
                .ToArray();
        }

        #endregion

        #region Helper methods

        private void InitSelectors()
        {
            Departments.Items.Clear();

            var filter = new DepartmentFilter
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier
            };

            if (!Identity.HasAccessToAllCompanies)
                filter.UserIdentifier = User.UserIdentifier;

            var departments = ContactRepository3.SelectDepartments(filter);

            foreach (DataRow row in departments.Rows)
                Departments.Items.Add(new System.Web.UI.WebControls.ListItem
                {
                    Value = row["DepartmentIdentifier"].ToString(),
                    Text = (string)row["DepartmentName"]
                });
        }

        private string GetDepartments()
        {
            var result = new StringBuilder();

            foreach (System.Web.UI.WebControls.ListItem item in Departments.Items)
            {
                if (!item.Selected)
                    continue;

                if (result.Length > 0)
                    result.Append(",");

                result.Append(item.Value);
            }

            if (result.Length == 0)
                foreach (System.Web.UI.WebControls.ListItem item in Departments.Items)
                {
                    if (result.Length > 0)
                        result.Append(",");

                    result.Append(item.Value);
                }

            return result.ToString();
        }

        #endregion
    }
}