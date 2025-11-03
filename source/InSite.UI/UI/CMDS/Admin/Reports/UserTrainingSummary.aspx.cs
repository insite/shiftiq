using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant.CMDS;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class UserTrainingSummary : AdminBasePage, ICmdsUserControl
    {
        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;

        private string[] _achievementCategories;
        private List<VCmdsAchievement> _achievements;
        private Dictionary<Guid, int> _achievementMap;
        private DataTable _employees;
        private Dictionary<Guid, int> _employeeMap;
        private string[,] _reportData;

        private StringBuilder _csv;

        private PersonFinderSecurityInfoWrapper FinderSecurityInfo => _finderSecurityInfo
            ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                FinderSecurityInfo.LoadPermissions();

                Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

                if (!Identity.HasAccessToAllCompanies)
                    Department.Filter.UserIdentifier = User.UserIdentifier;

                Department.Value = null;

                LoadAchievementCategories();
            }
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var csv = CreateReport();

            Response.SendFile("EmployeeTrainingSummaryReport.csv", Encoding.UTF8.GetBytes(csv));
        }

        private string CreateReport()
        {
            LoadReportData();

            _csv = new StringBuilder();

            AddReportHeader();
            AddTableHeader();
            AddReportData();

            return _csv.ToString();
        }

        private void AddReportHeader()
        {
            AddValueToCsv(Organization.CompanyName, false);

            for (int i = 0; i < _achievements.Count; i++)
                AddValueToCsv(null, true);

            _csv.AppendLine();

            if (Department.Value.HasValue)
                AddValueToCsv(DepartmentSearch.BindFirst(x => x.DepartmentName, x => x.DepartmentIdentifier == Department.Value), false);

            for (int i = 0; i < _achievements.Count; i++)
                AddValueToCsv(null, true);

            _csv.AppendLine();

            for (int i = 0; i < _achievements.Count + 1; i++)
                AddValueToCsv(null, i > 0);

            _csv.AppendLine();

            AddValueToCsv(string.Format("Report created on {0:ddd MMM d HH:mm:ss} UTC {0:yyyy}.", DateTime.UtcNow), false);

            for (int i = 0; i < _achievements.Count; i++)
                AddValueToCsv(null, true);

            _csv.AppendLine();

            for (int i = 0; i < _achievements.Count + 1; i++)
                AddValueToCsv(null, i > 0);

            _csv.AppendLine();
        }

        private void AddTableHeader()
        {
            AddValueToCsv(null, false);

            foreach (var row in _achievements)
                AddValueToCsv(row.AchievementTitle, true);

            _csv.AppendLine();

            AddValueToCsv("Renewal", false);

            foreach (var row in _achievements)
            {
                var validForCount = row.ValidForCount;
                var validForUnit = row.ValidForUnit;

                if (validForCount == null || string.IsNullOrEmpty(validForUnit))
                {
                    AddValueToCsv(null, true);
                }
                else
                {
                    string text = validForUnit == ValidForUnits.Years
                        ? string.Format("{0} yrs. 0 mos.", validForCount)
                        : string.Format("0 yrs. {0} mos.", validForCount);

                    AddValueToCsv(text, true);
                }
            }

            _csv.AppendLine();

            AddValueToCsv("Name", false);

            for (int i = 1; i < _achievements.Count; i++)
                AddValueToCsv("Expiry", true);

            _csv.AppendLine();
        }

        private void AddReportData()
        {
            for (int i = 0; i < _employees.Rows.Count; i++)
            {
                DataRow row = _employees.Rows[i];
                string name = (string)row["FullName"];

                AddValueToCsv(name, false);

                for (int k = 0; k < _achievements.Count; k++)
                    AddValueToCsv(_reportData[i, k], true);

                _csv.AppendLine();
            }
        }

        private void LoadReportData()
        {
            List<string> achievementCategoriesList = new List<string>();

            foreach (ListItem item in AchievementCategories.Items)
            {
                if (item.Selected)
                    achievementCategoriesList.Add(item.Value);
            }

            _achievementCategories = achievementCategoriesList.ToArray();

            LoadAchievements();
            LoadEmployees();
            LoadEmployeeAchievements();
        }

        private void LoadAchievements()
        {
            _achievements = Department.Value.HasValue
                ? VCmdsCredentialSearch.SelectAchievementsByDepartment(new[] { Department.Value.Value }, _achievementCategories, GetIsRequired())
                : VCmdsCredentialSearch.SelectAchievementsByCompany(CurrentIdentityFactory.ActiveOrganizationIdentifier, _achievementCategories, GetIsRequired());

            _achievementMap = new Dictionary<Guid, int>();

            for (int i = 0; i < _achievements.Count; i++)
            {
                var row = _achievements[i];
                if (!_achievementMap.ContainsKey(row.AchievementIdentifier))
                    _achievementMap.Add(row.AchievementIdentifier, i);
            }
        }

        private void LoadEmployees()
        {
            _employees = Department.Value.HasValue
                ? ContactRepository3.SelectEmployeesByDepartmentId(Department.Value.Value)
                : ContactRepository3.SelectEmployeesByOrganizationIdentifier(CurrentIdentityFactory.ActiveOrganizationIdentifier);

            _employeeMap = new Dictionary<Guid, int>();

            for (int i = 0; i < _employees.Rows.Count; i++)
            {
                DataRow row = _employees.Rows[i];
                Guid employeeID = (Guid)row["UserIdentifier"];

                if (!_employeeMap.ContainsKey(employeeID))
                    _employeeMap.Add(employeeID, i);
            }
        }

        private void LoadEmployeeAchievements()
        {
            var credentials = Department.Value.HasValue
                ? VCmdsCredentialSearch.SelectEmployeeAchievementsByDepartment(Department.Value.Value, _achievementCategories, GetIsRequired())
                : VCmdsCredentialSearch.SelectEmployeeAchievementsByCompany(CurrentIdentityFactory.ActiveOrganizationIdentifier, _achievementCategories, GetIsRequired());

            _reportData = new string[_employees.Rows.Count, _achievements.Count];

            foreach (var row in credentials)
            {
                var expirationDate = row.CredentialExpirationExpected;

                if (expirationDate == null)
                    continue;

                var employeeID = row.UserIdentifier;
                var achievementIdentifier = row.AchievementIdentifier;

                var expiredText = expirationDate.Value <= DateTime.UtcNow.Date ? " (Expired)" : null;
                var text = string.Format("{0:MMM d, yyyy}{1}", expirationDate, expiredText);

                var employeeIndex = _employeeMap[employeeID];
                var achievementIndex = _achievementMap[achievementIdentifier];

                _reportData[employeeIndex, achievementIndex] = text;
            }
        }

        private void LoadAchievementCategories()
        {
            var labels = ServiceLocator.AchievementSearch.GetAchievementLabels(Organization.Identifier);
            var types = VCmdsAchievementSearch.SelectAchievementLabels(Organization.Code, labels, null);

            foreach (var type in types.Items)
            {
                AchievementCategories.Items.Add(new ListItem(type.Value, type.Value));
            }
        }

        private void AddValueToCsv(object value, bool addSeparator)
        {
            string s = value == DBNull.Value || value == null ? null : value.ToString();

            s = string.IsNullOrEmpty(s) ? null : string.Format("\"{0}\"", s.Replace("\"", "\"\""));

            if (addSeparator)
                _csv.AppendFormat("{0}{1}", ',', s);
            else
                _csv.Append(s);
        }

        private bool? GetIsRequired()
        {
            return IsRequired.SelectedIndex > 0 ? bool.Parse(IsRequired.SelectedValue) : (bool?)null;
        }
    }
}
