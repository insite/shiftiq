using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Cmds.Actions.Reports
{
    public partial class CompetencyStatusPerPerson : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        private class EmployeeItem
        {
            public string Name { get; set; }
            public int? Expired { get; set; }
            public int? NeedsTraining { get; set; }
            public int? NotApplicable { get; set; }
            public int? NotCompleted { get; set; }
            public int? SelfAssessed { get; set; }
            public int? Submitted { get; set; }
            public int? Validated { get; set; }
            public int Total { get; set; }
        }

        private class GroupItem
        {
            public string GroupName { get; set; }
            public List<EmployeeItem> Employees { get; set; }
        }

        [Serializable]
        private class SearchParameters
        {
            public Guid? DepartmentIdentifier { get; set; }
            public Guid? PersonIdentifier { get; set; }
            public Guid? ProfileIdentifier { get; set; }
            public bool IsPrimaryProfile { get; set; }
            public IEnumerable<string> EmploymentTypes { get; set; }
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

        protected void Page_Init(object sender, EventArgs e)
        {
            DepartmentPersonValidator.ServerValidate +=
                (s, a) => a.IsValid = DepartmentIdentifier.HasValue || PersonIdentifier.HasValue;

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) => UpdatePersonIdentifier();

            EmploymentTypes.AutoPostBack = true;
            EmploymentTypes.SelectedIndexChanged += (s, a) => UpdatePersonIdentifier();

            ProfileMode.AutoPostBack = true;
            ProfileMode.ValueChanged += (s, a) => ProfileField.Visible = ProfileMode.Value == "Specific";

            ReportButton.Click += ReportButton_Click;

            DownloadXlsx.Click += DownloadXlsx_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            DepartmentIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            DepartmentIdentifier.Value = null;

            PersonIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            UpdatePersonIdentifier();

            ProfileIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            ProfileIdentifier.Value = null;
        }

        private void UpdatePersonIdentifier()
        {
            PersonIdentifier.Filter.DepartmentIdentifier = DepartmentIdentifier.Value;
            PersonIdentifier.Filter.RoleType = GetEmploymentTypes().ToArray().NullIfEmpty();
            PersonIdentifier.Value = null;
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            var dataSource = CreateGroups(CurrentParameters);
            var helper = new XlsxExportHelper();

            helper.Map("Name", "Name", null, 30, HorizontalAlignment.Left);
            helper.Map("Expired", "Expired", "#,##0", 20, HorizontalAlignment.Right);
            helper.Map("NeedsTraining", "Needs Training", "#,##0", 20, HorizontalAlignment.Right);
            helper.Map("NotApplicable", "Not Applicable", "#,##0", 20, HorizontalAlignment.Right);
            helper.Map("NotCompleted", "Not Completed", "#,##0", 20, HorizontalAlignment.Right);
            helper.Map("SelfAssessed", "Self-Assessed", "#,##0", 20, HorizontalAlignment.Right);
            helper.Map("Submitted", "Submitted for Validation", "#,##0", 20, HorizontalAlignment.Right);
            helper.Map("Validated", "Validated", "#,##0", 20, HorizontalAlignment.Right);
            helper.Map("Total", "Total", "#,##0", 20, HorizontalAlignment.Right);

            var bytes = helper.GetXlsxBytes(excel =>
            {
                var sheet = excel.Workbook.Worksheets.Add(Route.Title);
                sheet.Cells.Style.WrapText = true;

                var row = 1;

                foreach (var group in dataSource)
                {
                    var groupCells = sheet.Cells[row, 1, row, 9];
                    groupCells.Merge = true;
                    groupCells.Value = group.GroupName;
                    groupCells.StyleName = XlsxExportHelper.HeaderStyleName;

                    row++;

                    helper.InsertHeader(sheet, row, 1);

                    row++;

                    row += helper.InsertData(sheet, group.Employees, row, 1);

                    var separatorCells = sheet.Cells[row, 1, row, 9];
                    separatorCells.Merge = true;

                    row++;
                }

                helper.ApplyColumnWidth(sheet, 1, true);
            });


            ReportXlsxHelper.ExportToXlsx(Route.Title, bytes);
        }

        #endregion

        #region Helper methods

        private void LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                DepartmentIdentifier = DepartmentIdentifier.Value,
                PersonIdentifier = PersonIdentifier.Value,
                ProfileIdentifier = ProfileMode.Value == "Specific" ? ProfileIdentifier.Value : null,
                IsPrimaryProfile = ProfileMode.Value == "Primary",
                EmploymentTypes = GetEmploymentTypes()
            };

            var groups = CreateGroups(CurrentParameters);

            if (!groups.Any())
            {
                ScreenStatus.AddMessage(AlertType.Information, "There is no data matching your criteria.");
                return;
            }

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            foreach (var group in groups)
            {
                ReportOutput.Controls.Add(new HtmlGenericControl
                {
                    TagName = "h3",
                    InnerHtml = group.GroupName
                });

                var gv = new GridView
                {
                    AutoGenerateColumns = false,
                    CssClass = "table table-striped"
                };

                gv.Columns.Add(new System.Web.UI.WebControls.BoundField { HeaderText = "Name", DataField = "Name" });
                gv.Columns.Add(GetNumericBoundField("Expired", "Expired"));
                gv.Columns.Add(GetNumericBoundField("Needs Training", "NeedsTraining"));
                gv.Columns.Add(GetNumericBoundField("Not Applicable", "NotApplicable"));
                gv.Columns.Add(GetNumericBoundField("Not Completed", "NotCompleted"));
                gv.Columns.Add(GetNumericBoundField("Self-Assessed", "SelfAssessed"));
                gv.Columns.Add(GetNumericBoundField("Submitted for Validation", "Submitted"));
                gv.Columns.Add(GetNumericBoundField("Validated", "Validated"));
                gv.Columns.Add(GetNumericBoundField("Total", "Total"));

                gv.DataSource = group.Employees;
                gv.DataBind();

                gv.HeaderRow.TableSection = TableRowSection.TableHeader;

                ReportOutput.Controls.Add(gv);
            }
        }

        private static int? GetStatusCount(ContactRepository2.Employment employment, string status)
        {
            return employment.ValidationStatuses.FirstOrDefault(x => x.Status.Equals(status, StringComparison.OrdinalIgnoreCase))?.Count;
        }

        private static System.Web.UI.WebControls.BoundField GetNumericBoundField(string headerText, string dataField)
        {
            var boundField = new System.Web.UI.WebControls.BoundField
            {
                HeaderText = headerText,
                DataField = dataField,
                DataFormatString = "{0:n0}"
            };

            boundField.HeaderStyle.CssClass = "text-end";
            boundField.ItemStyle.CssClass = "text-end";

            return boundField;
        }

        private static IEnumerable<GroupItem> CreateGroups(SearchParameters parameters)
        {
            var data = ApplyFilter(parameters);

            var dictCompAndDep = new Dictionary<string, GroupItem>();

            foreach (var employee in data)
            {
                foreach (var employment in employee.Employments)
                {
                    var employeeItem = new EmployeeItem
                    {
                        Name = employee.Name,
                        Expired = GetStatusCount(employment, ValidationStatuses.Expired),
                        NeedsTraining = GetStatusCount(employment, ValidationStatuses.NeedsTraining),
                        NotApplicable = GetStatusCount(employment, ValidationStatuses.NotApplicable),
                        NotCompleted = GetStatusCount(employment, ValidationStatuses.NotCompleted),
                        SelfAssessed = GetStatusCount(employment, ValidationStatuses.SelfAssessed),
                        Submitted = GetStatusCount(employment, ValidationStatuses.SubmittedForValidation),
                        Validated = GetStatusCount(employment, ValidationStatuses.Validated),
                        Total = employment.ValidationStatuses.Sum(x => x.Count)
                    };

                    var groupName = string.Format("{0} - {1}", employment.Company, employment.Department);

                    if (!dictCompAndDep.ContainsKey(groupName))
                        dictCompAndDep.Add(groupName, new GroupItem
                        {
                            GroupName = groupName,
                            Employees = new List<EmployeeItem> { employeeItem }
                        });
                    else
                        dictCompAndDep[groupName].Employees.Add(employeeItem);
                }
            }

            var keys = dictCompAndDep.Keys.OrderBy(k => k);
            var groups = new List<GroupItem>();

            foreach (var key in keys)
            {
                var group = dictCompAndDep[key];
                group.Employees.Sort((a, b) => a.Name.CompareTo(b.Name));

                groups.Add(group);
            }

            return groups;
        }

        private static IEnumerable<ContactRepository2.CompanyEmployee> ApplyFilter(SearchParameters parameters)
        {
            var statuses = ContactRepository2.SelectCompetencyStatusPerPerson(
                CurrentIdentityFactory.ActiveOrganizationIdentifier,
                parameters.DepartmentIdentifier,
                parameters.PersonIdentifier,
                parameters.ProfileIdentifier,
                parameters.IsPrimaryProfile);

            statuses = statuses.Where(x => x.Employments.Any(e => e.EmploymentType != null && parameters.EmploymentTypes.Contains(e.EmploymentType)));

            var departments = MembershipSearch.Bind(
                x => x.Group.GroupName,
                x => x.Group.GroupType == GroupTypes.Department
                  && x.UserIdentifier == User.UserIdentifier);

            if (!Identity.IsInGroup(CmdsRole.SystemAdministrators) && !Identity.IsInGroup(CmdsRole.Programmers))
                statuses = statuses.Where(
                    s => s.Employments.Any(
                        e => departments.Any(
                            d => d.Equals(e.Department, StringComparison.OrdinalIgnoreCase))));

            return statuses;
        }

        private IEnumerable<string> GetEmploymentTypes()
        {
            var list = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (EmploymentTypes.Items.FindByValue("Administration").Selected)
                list.Add(MembershipType.Administration);

            if (EmploymentTypes.Items.FindByValue("Organization").Selected)
                list.Add(MembershipType.Organization);

            if (EmploymentTypes.Items.FindByValue("Department").Selected)
                list.Add(MembershipType.Department);

            return list;
        }

        #endregion
    }
}