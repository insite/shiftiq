using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;
using Shift.Sdk.UI;
using Shift.Toolbox;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class DepartmentProfileDetail : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        protected class SearchParameters
        {
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public string ProfileMode { get; set; }
            public Guid? ProfileIdentifier { get; set; }
        }

        private class ReportData
        {
            public List<CompetencyItem> Competencies { get; set; }
            public Dictionary<Guid, int> CompetencyMap { get; set; }
            public Dictionary<EmployeeKey, int> EmployeeMap { get; set; }
            public DataTable Employees { get; set; }
            public Guid[] Profiles { get; set; }
            public string[,] Statuses { get; set; }
            public ManagerDetailsReportLevelSummary Level { get; set; }
        }

        private class EmployeeKey : MultiKey<Guid, Guid>
        {
            public EmployeeKey(Guid user, Guid profile) : base(user, profile)
            {
            }
        }

        private class CompetencyItem
        {
            #region Fields

            private readonly List<string> _priorities = new List<string>();

            #endregion

            #region Properties

            public string Number { get; set; }
            public string Category { get; set; }
            public string Summary { get; set; }

            #endregion

            #region Methods

            public void AddPriorityName(string priorityName)
            {
                if (!string.IsNullOrEmpty(priorityName) && !_priorities.Contains(priorityName))
                    _priorities.Add(priorityName);
            }

            public string PrioritiesToText()
            {
                var text = new StringBuilder();

                foreach (var priorityName in _priorities)
                {
                    if (text.Length != 0)
                        text.Append(", ");

                    text.Append(priorityName);
                }

                return text.ToString();
            }

            #endregion
        }

        private class ManagerDetailsReportStatusSummary
        {
            public ManagerDetailsReportStatusSummary(string statusName, int employeeCount)
            {
                StatusName = statusName;
                Counts = new int[employeeCount];
            }

            public string StatusName { get; }
            public int[] Counts { get; }
        }

        private class ManagerDetailsReportLevelSummary
        {
            public ManagerDetailsReportLevelSummary(int employeeCount)
            {
                StatusList = new ManagerDetailsReportStatusSummary[7];
                StatusList[0] = new ManagerDetailsReportStatusSummary(ValidationStatuses.NotCompleted, employeeCount);
                StatusList[1] = new ManagerDetailsReportStatusSummary(ValidationStatuses.SelfAssessed, employeeCount);
                StatusList[2] = new ManagerDetailsReportStatusSummary(ValidationStatuses.SubmittedForValidation, employeeCount);
                StatusList[3] = new ManagerDetailsReportStatusSummary(ValidationStatuses.Validated, employeeCount);
                StatusList[4] = new ManagerDetailsReportStatusSummary(ValidationStatuses.NeedsTraining, employeeCount);
                StatusList[5] = new ManagerDetailsReportStatusSummary(ValidationStatuses.Expired, employeeCount);
                StatusList[6] = new ManagerDetailsReportStatusSummary(ValidationStatuses.NotApplicable, employeeCount);
            }

            public ManagerDetailsReportStatusSummary[] StatusList { get; }

            public ManagerDetailsReportStatusSummary this[string statusName]
            {
                get
                {
                    foreach (var statusSummary in StatusList)
                        if (StringHelper.Equals(statusSummary.StatusName, statusName))
                            return statusSummary;

                    return null;
                }
            }
        }

        #endregion

        #region Security

        protected SearchParameters ReportParameters
        {
            get => (SearchParameters)ViewState[nameof(ReportParameters)];
            set => ViewState[nameof(ReportParameters)] = value;
        }

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) => UpdateProfileIdentifier();

            ProfileMode.AutoPostBack = true;
            ProfileMode.SelectedIndexChanged += ProfileMode_SelectedIndexChanged;

            ReportButton.Click += ReportButton_Click;
            DownloadXlsx1.Click += DownloadXlsx1_Click;
            DownloadXlsx2.Click += DownloadXlsx2_Click;

            DataRepeater.ItemDataBound += DataRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            InitSelectors();
        }

        private void InitSelectors()
        {
            DepartmentIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!Identity.HasAccessToAllCompanies)
                DepartmentIdentifier.Filter.UserIdentifier = User.UserIdentifier;

            DepartmentIdentifier.Value = null;

            UpdateProfileIdentifier();
        }

        private void UpdateProfileIdentifier()
        {
            ProfileIdentifier.Value = null;
            ProfileIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            ProfileIdentifier.Filter.DepartmentIdentifier = DepartmentIdentifier.Value;
        }

        #endregion

        #region Event handlers

        private void ProfileMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProfileField.Visible = ProfileMode.SelectedValue == "Specific";
            ProfileIdentifier.Value = null;
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void DownloadXlsx1_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                DownloadXlsx(CreateParameters());
        }

        private void DownloadXlsx2_Click(object sender, EventArgs e)
        {
            DownloadXlsx(ReportParameters);
        }

        private void DataRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var rowLiteral = (ITextControl)e.Item.FindControl("RowLiteral");
            var dataRow = (DataRowView)e.Item.DataItem;
            var row = new StringBuilder();

            for (var i = 0; i < dataRow.Row.Table.Columns.Count; i++)
            {
                var value = dataRow[i].ToString();
                row.Append($"<td>{value}</td>");
            }

            rowLiteral.Text = row.ToString();
        }

        #endregion

        #region Create report

        private SearchParameters CreateParameters()
        {
            var departmentItem = DepartmentIdentifier.Item;

            return new SearchParameters
            {
                DepartmentIdentifier = departmentItem.Value,
                DepartmentName = departmentItem.Text,
                ProfileMode = ProfileMode.SelectedValue,
                ProfileIdentifier = ProfileIdentifier.Value
            };
        }

        private static DataTable CreateReportData(SearchParameters parameters)
        {
            var data = new ReportData();

            LoadEmployeesAndProfiles(parameters, data);
            LoadCompetencies(parameters, data);

            if (data.Competencies.IsEmpty())
                return null;

            data.Statuses = new string[data.Competencies.Count, data.Employees.Rows.Count];

            LoadStatuses(parameters, data);

            CalculateSummary(data);

            var result = new DataTable();
            result.Columns.Add("Organization", typeof(string));
            result.Columns.Add("Competency", typeof(string));
            result.Columns.Add("Type", typeof(string));
            result.Columns.Add("Criticality", typeof(string));
            result.Columns.Add("CompetencySummary", typeof(string));

            foreach (DataRow row in data.Employees.Rows)
            {
                var columnName = GetColumnName(row);
                var columns = result.Columns;
                if (!columns.Contains(columnName))
                    columns.Add(columnName);
            }

            AddEmployeesToReportData(parameters, data, result);
            AddStatusesToReportData(data, result);
            AddSummaryToReportData(data, result);

            return result;
        }

        private void DownloadXlsx(SearchParameters parameters)
        {
            var dataSource = CreateReportData(parameters);

            if (dataSource == null || dataSource.Rows.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            var helper = new XlsxExportHelper();
            helper.Map(dataSource.Columns[0].ColumnName, "None", null, 25, HorizontalAlignment.Left);
            helper.Map(dataSource.Columns[1].ColumnName, "None", null, 15, HorizontalAlignment.Left);
            helper.Map(dataSource.Columns[2].ColumnName, "None", null, 10, HorizontalAlignment.Left);
            helper.Map(dataSource.Columns[3].ColumnName, "None", null, 15, HorizontalAlignment.Left);
            helper.Map(dataSource.Columns[4].ColumnName, "None", null, 60, HorizontalAlignment.Left);

            for (var i = 5; i < dataSource.Columns.Count; i++)
                helper.Map(dataSource.Columns[i].ColumnName, "None", null, 20, HorizontalAlignment.Left);

            var bytes = helper.GetXlsxBytes(dataSource, Route.Title, false);

            ReportXlsxHelper.ExportToXlsx(Route.Title, bytes);
        }

        private void LoadReport()
        {
            ReportParameters = CreateParameters();

            var dataSource = CreateReportData(ReportParameters);
            if (dataSource == null || dataSource.Rows.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There is no data matching your criteria.");
                return;
            }

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DataRepeater.DataSource = dataSource;
            DataRepeater.DataBind();
        }

        #endregion

        #region Add Emploees

        private static void AddEmployeesToReportData(SearchParameters parameters, ReportData data, DataTable table)
        {
            AddEmployeeNamesToReportData(data, table);
            AddEmployeeProfilesToReportData(parameters, data, table);
            AddProfileStatusesToReportData(data, table);
            AddEmployeeIsPrimaryProfilesToReportData(data, table);
        }

        private static void AddEmployeeNamesToReportData(ReportData data, DataTable table)
        {
            var reportRow = table.NewRow();
            reportRow["Organization"] = Organization.CompanyName;

            foreach (DataRow row in data.Employees.Rows)
            {
                var columnName = GetColumnName(row);
                reportRow[columnName] = row["PersonFullName"];
            }

            table.Rows.Add(reportRow);
        }

        private static void AddEmployeeProfilesToReportData(SearchParameters parameters, ReportData data, DataTable table)
        {
            var reportRow = table.NewRow();
            reportRow["Organization"] = parameters.DepartmentName;
            reportRow["CompetencySummary"] = "Profile ->";

            foreach (DataRow row in data.Employees.Rows)
            {
                var columnName = GetColumnName(row);
                reportRow[columnName] = row["ProfileNumber"];
            }

            table.Rows.Add(reportRow);
        }

        private static void AddProfileStatusesToReportData(ReportData data, DataTable table)
        {
            var reportRow = table.NewRow();
            reportRow["Organization"] = DateTime.Now.Format();
            reportRow["CompetencySummary"] = "Status ->";

            foreach (DataRow row in data.Employees.Rows)
            {
                var columnName = GetColumnName(row);
                reportRow[columnName] = row["ProfileStatusName"];
            }

            table.Rows.Add(reportRow);
        }

        private static void AddEmployeeIsPrimaryProfilesToReportData(ReportData data, DataTable table)
        {
            var reportRow = table.NewRow();
            reportRow["CompetencySummary"] = "Primary ->";

            foreach (DataRow row in data.Employees.Rows)
            {
                var columnName = GetColumnName(row);
                reportRow[columnName] = (bool)row["IsPrimaryProfile"] ? "Yes" : "No";
            }

            table.Rows.Add(reportRow);
        }

        #endregion

        #region Add Statuses

        private static void AddStatusesToReportData(ReportData data, DataTable table)
        {
            var headerRow = table.NewRow();

            headerRow["Competency"] = "Competency";
            headerRow["Type"] = "Type";
            headerRow["Criticality"] = "Criticality";
            headerRow["CompetencySummary"] = "Competency Summary";

            for (var i = 0; i < data.Employees.Rows.Count; i++)
            {
                var row = data.Employees.Rows[i];
                var columnName = GetColumnName(row);
                headerRow[columnName] = "Status";
            }

            table.Rows.Add(headerRow);

            for (var competencyIndex = 0; competencyIndex < data.Competencies.Count; competencyIndex++)
            {
                var competency = data.Competencies[competencyIndex];

                var reportRow = table.NewRow();
                reportRow["Competency"] = competency.Number;
                reportRow["Type"] = competency.Category;
                reportRow["Criticality"] = competency.PrioritiesToText();
                reportRow["CompetencySummary"] = competency.Summary;

                for (var employeeIndex = 0; employeeIndex < data.Employees.Rows.Count; employeeIndex++)
                {
                    var validationStatus = data.Statuses[competencyIndex, employeeIndex];

                    var row = data.Employees.Rows[employeeIndex];
                    var columnName = GetColumnName(row);
                    reportRow[columnName] = GetStatusAbbrev(validationStatus);
                }

                table.Rows.Add(reportRow);
            }
        }

        #endregion

        #region Load report data

        private static void LoadEmployeesAndProfiles(SearchParameters parameters, ReportData data)
        {
            var filter = new PersonFilter
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                DepartmentIdentifier = parameters.DepartmentIdentifier,
                IsApproved = true,
                RoleType = new[] { MembershipType.Department }
            };

            var profileFilter = new PersonProfileFilter();

            switch (parameters.ProfileMode)
            {
                case "Primary":
                    profileFilter.OnlyPrimaryProfile = true;
                    break;
                case "Compliance":
                    profileFilter.OnlyComplianceRequired = true;
                    break;
                case "Specific":
                    profileFilter.ProfileStandardIdentifier = parameters.ProfileIdentifier.Value;
                    break;
            }

            data.Employees = ContactRepository3.SelectSearchResultsWithProfiles(filter, profileFilter, Organization.Identifier);
            data.Profiles = data.Employees.AsEnumerable().Select(x => (Guid)x["ProfileStandardIdentifier"]).Distinct().ToArray();

            data.EmployeeMap = new Dictionary<EmployeeKey, int>();

            for (var i = 0; i < data.Employees.Rows.Count; i++)
            {
                var row = data.Employees.Rows[i];
                var userKey = (Guid)row["UserIdentifier"];
                var profileStandardIdentifier = (Guid)row["ProfileStandardIdentifier"];

                data.EmployeeMap.Add(new EmployeeKey(userKey, profileStandardIdentifier), i);
            }
        }

        private static void LoadCompetencies(SearchParameters parameters, ReportData data)
        {
            var filter = new CompetencyFilter { Profiles = data.Profiles };

            var table = CompetencyRepository.SelectSearchResultsWithDepartment(filter, parameters.DepartmentIdentifier);

            data.Competencies = new List<CompetencyItem>();
            data.CompetencyMap = new Dictionary<Guid, int>();

            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var id = (Guid)row["CompetencyStandardIdentifier"];
                var priorityName = row["PriorityName"] as string;

                if (data.CompetencyMap.TryGetValue(id, out var competencyIndex))
                {
                    var competency = data.Competencies[competencyIndex];
                    competency.AddPriorityName(priorityName);
                }
                else
                {
                    var competency = new CompetencyItem
                    {
                        Number = row["Number"] as string,
                        Category = row["Category"] as string,
                        Summary = row["Summary"] as string
                    };
                    competency.AddPriorityName(priorityName);

                    data.Competencies.Add(competency);
                    data.CompetencyMap.Add(id, data.Competencies.Count - 1);
                }
            }
        }

        private static void LoadStatuses(SearchParameters parameters, ReportData data)
        {
            if (parameters.ProfileMode == "Specific")
                LoadStatusesForSpecificProfile(parameters.ProfileIdentifier.Value, null, parameters, data);
            else
                LoadStatusesForProfiles(parameters, data);
        }

        private static void LoadStatusesForProfiles(SearchParameters parameters, ReportData data)
        {
            foreach (DataRow row in data.Employees.Rows)
            {
                var profileID = (Guid)row["ProfileStandardIdentifier"];
                var employeeID = (Guid)row["UserIdentifier"];

                LoadStatusesForSpecificProfile(profileID, employeeID, parameters, data);
            }
        }

        private static void LoadStatusesForSpecificProfile(Guid profileStandardIdentifier, Guid? userKey, SearchParameters parameters, ReportData data)
        {
            var filter = new EmployeeCompetencyFilter
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                DepartmentIdentifier = parameters.DepartmentIdentifier,
                ProfileStandardIdentifier = profileStandardIdentifier,
                UserIdentifier = userKey
            };

            var table = UserCompetencyRepository.SelectSearchResults(filter, null, null);

            foreach (DataRow row in table.Rows)
            {
                var competencyID = (Guid)row["CompetencyStandardIdentifier"];
                if (!data.CompetencyMap.TryGetValue(competencyID, out var competencyIndex))
                    continue;

                var currentEmployeeID = (Guid)row["UserIdentifier"];
                var validationStatus = (string)row["ValidationStatus"];

                var employeeKey = new EmployeeKey(currentEmployeeID, profileStandardIdentifier);
                if (data.EmployeeMap.TryGetValue(employeeKey, out var employeeIndex))
                    data.Statuses[competencyIndex, employeeIndex] = validationStatus;
            }
        }

        #endregion

        #region Helper methods

        private static string GetColumnName(DataRow row)
        {
            return $"Employee{row["UserIdentifier"]}_Profile{row["ProfileStandardIdentifier"]}";
        }

        private static string GetStatusAbbrev(string status)
        {
            if (StringHelper.Equals(status, ValidationStatuses.Expired))
                return "Exp.";

            if (StringHelper.Equals(status, ValidationStatuses.SubmittedForValidation))
                return "Sub.";

            if (StringHelper.Equals(status, ValidationStatuses.NotCompleted))
                return "N.C.";

            if (StringHelper.Equals(status, ValidationStatuses.NotApplicable))
                return "N.A.";

            if (StringHelper.Equals(status, ValidationStatuses.NeedsTraining))
                return "N.T.";

            if (StringHelper.Equals(status, ValidationStatuses.SelfAssessed))
                return "S.A.";

            if (StringHelper.Equals(status, ValidationStatuses.Validated))
                return "Val.";

            return null;
        }

        #endregion

        #region Summary

        private static void AddSummaryToReportData(ReportData data, DataTable table)
        {
            table.Rows.Add(table.NewRow());

            foreach (var statusSummary in data.Level.StatusList)
            {
                var reportRow = table.NewRow();

                var abbrev = GetStatusAbbrev(statusSummary.StatusName);

                var name = string.IsNullOrEmpty(abbrev)
                    ? statusSummary.StatusName
                    : string.Format("{0} = {1}", abbrev, statusSummary.StatusName);

                reportRow["Competency"] = name;

                for (var i = 0; i < data.Employees.Rows.Count; i++)
                {
                    var count = statusSummary.Counts[i];
                    var row = data.Employees.Rows[i];
                    var columnName = GetColumnName(row);
                    reportRow[columnName] = count;
                }

                table.Rows.Add(reportRow);
            }
        }

        private static void CalculateSummary(ReportData data)
        {
            data.Level = new ManagerDetailsReportLevelSummary(data.Employees.Rows.Count);

            for (var competencyIndex = 0; competencyIndex < data.Competencies.Count; competencyIndex++)
                for (var employeeIndex = 0; employeeIndex < data.Employees.Rows.Count; employeeIndex++)
                {
                    var statusName = data.Statuses[competencyIndex, employeeIndex];
                    var statusSummary = data.Level[statusName];

                    if (statusSummary != null)
                        statusSummary.Counts[employeeIndex]++;
                }
        }

        #endregion
    }
}